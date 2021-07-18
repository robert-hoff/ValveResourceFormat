using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace TestMyVRF {


    class ParseDynamicExpressions {

        // if successful the parsed data assigned here
        public string dynamicExpressionResult = "";
        // parse the input one line at a time, we track certain conditions to delimit these
        public List<string> dynamicExpressionList = new List<string>();

        // if we have problems parsing, no change is made to the input, so fall back
        // on binary data to represent the KV-field
        public bool errorWhileParsing;
        public string errorMessage = "";


        // function reference, name and number of arguments (some functions are unknown)
        private (string, int)[] FUNCTION_REF = {
            ("sin",     1),     // 00
            ("cos",     1),     // 01
            ("tan",     1),     // 02
            ("frac",    1),     // 03
            ("floor",   1),     // 04
            ("ceil",    1),     // 05
            ("?",      -1),     // 06
            ("?",      -1),     // 07
            ("lerp",    3),     // 08
            ("?",      -1),     // 09
            ("?",      -1),     // 0A
            ("?",      -1),     // 0B
            ("log",     1),     // 0C
            ("log2",    1),     // 0D
            ("log10",   1),     // 0E
            ("exp",     1),     // 0F
            ("?",      -1),     // 10
            ("sqrt",    1),     // 11
            ("?",      -1),     // 12
            ("sign",    1),     // 13
            ("abs",     1),     // 14
            ("pow",     2),     // 15
            ("step",    2),     // 16
            ("?",      -1),     // 17
            ("float4",  4),     // 18
            ("float3",  3),     // 19
            ("float2",  2),     // 1A
            ("time",    0),     // 1B
            ("min",     2),     // 1C
            ("max",     2),     // 1D
            ("?",      -1),     // 1E
            ("?",      -1),     // 1F
            ("random",  2),     // 20
        };


        private static string[] operatorSymbols = {
            "","","","","","","","","","","","","",
            "==","!=",">",">=","<","<=","+","-","*","/","%"};


        private enum OPS {
            ENDOFDATA = 0x00,
            BRANCH_SEP = 0x02,
            BRANCH = 0x04,
            FUNC = 0x06,
            FLOAT = 0x07,
            ASSIGN = 0x08,
            LOCALVAR = 0x09,
            NOT = 0x0C,
            EQUALS = 0x0D,               // 0D		==					13
            NEQUALS = 0x0E,              // 0E		!=					14
            GT = 0x0F,                   // 0F		> 					15
            GTE = 0x10,                  // 10		>=					16
            LT = 0x11,                   // 11		< 					17
            LTE = 0x12,                  // 12		<=					18
            ADD = 0x13,                  // 13		+					19
            SUB = 0x14,                  // 14		-					20
            MUL = 0x15,                  // 15		*					21
            DIV = 0x16,                  // 16		/					22
            MODULO = 0x17,               // 17		%					23
            NEGATION = 0x18,
            EXTVAR = 0x19,
            SWIZZLE = 0x1E,
            // NOT_AN_OPS = 0xff,
        };

        private Stack<string> expressions = new Stack<string>();
        private DataReader datareader;

        // check on each OPS if we are exiting a branch,
        // when we do we should combine expressions on the stack
        public Stack<uint> offsetAtBranchExits = new Stack<uint>();

        public ParseDynamicExpressions(byte[] binary_blob) {
            datareader = new DataReader(binary_blob);
            offsetAtBranchExits.Push(0);

            while (datareader.hasData()) {
                try {
                    ProcessOps((OPS)datareader.nextByte());
                } catch (System.ArgumentOutOfRangeException) {
                    errorWhileParsing = true;
                    errorMessage = "Parsing error - reader exceeded input";
                    Debug.WriteLine("EXCEPTION WAS THROWN");
                }

                if (errorWhileParsing) {
                    return;
                }


                // break;
            }

        }


        private void ProcessOps(OPS op) {

            //
            // combine the last three expressions on the stack into one,
            // on a branch exit we can look back at
            //
            //      <condition> ? <p1> : <p2>
            //
            // <condition>,<p1> and <p2> are always exactly 1 expression each
            //
            //
            if (offsetAtBranchExits.Peek() == datareader.offset) {
                offsetAtBranchExits.Pop();
                if (expressions.Count < 3) {
                    errorWhileParsing = true;
                    errorMessage = "error - not on a branch exit";
                    return;
                }
                string exp3 = expressions.Pop();
                string exp2 = expressions.Pop();
                string exp1 = expressions.Pop();
                string exp_conditional = $"({exp1}?{exp2}:{exp3})";
                expressions.Push(exp_conditional);
            }

            if (op == OPS.BRANCH_SEP) {
                uint branch_exit = datareader.nextInt16();
                offsetAtBranchExits.Push(branch_exit + 1);
                return;
            }

            // we will need the branch exit, it becomes available when we get to the branch separator
            // (in the middle of the conditional structure)
            if (op == OPS.BRANCH) {
                // some offsets here but we don't need them
                datareader.offset += 4;
                return;
            }

            if (op == OPS.EXTVAR) {
                uint varId = datareader.nextInt();
                string ext_varname = getExternalVarName(varId);
                expressions.Push(ext_varname);
                return;
            }

            if (op == OPS.FLOAT) {
                float float_val = datareader.nextFloat();
                string float_literal = string.Format("{0:g4}", float_val);
                // if a float leads with "0." remove the 0 (as how Valve likes it)
                if (float_literal.Length > 1 && float_literal.Substring(0, 2) == "0.") {
                    float_literal = float_literal.Substring(1);
                }
                expressions.Push(float_literal);
                return;
            }

            if (op == OPS.FUNC) {
                byte funcId = datareader.nextByte();
                byte funcCheckByte = datareader.nextByte();
                if (funcId > 0x20) {
                    errorWhileParsing = true;
                    errorMessage = $"Parsing error - invalid function Id = {funcId:x}";
                    return;
                }
                if (funcCheckByte != 0) {
                    errorWhileParsing = true;
                    errorMessage = $"Parsing error - malformed data";
                    return;
                }
                string funcName = FUNCTION_REF[funcId].Item1;
                int nrArguments = FUNCTION_REF[funcId].Item2;

                if (nrArguments == -1) {
                    errorWhileParsing = true;
                    errorMessage = $"Parsing error - this function ID has not been implemented = {funcId:x}";
                    return;
                }

                if (nrArguments > expressions.Count) {
                    errorWhileParsing = true;
                    errorMessage = $"Parsing error - the number of arguments for the function exceeds available expressions!";
                    return;
                }

                processFunction(funcName, nrArguments);
                return;
            }

            // R: I believe assignment is always to a local variable, and it terminates the line (check a bit more)
            if (op == OPS.ASSIGN) {
                byte varId = datareader.nextByte();
                string loc_varname = getLocalVarName(varId);
                string exp = expressions.Pop();
                string final_expression = $"{loc_varname} = {trimb(exp)};";
                Debug.WriteLine(final_expression);
                dynamicExpressionList.Add(final_expression);
                return;
            }

            if (op == OPS.LOCALVAR) {
                byte varId = datareader.nextByte();
                string loc_varname = getLocalVarName(varId);
                expressions.Push(loc_varname);
                return;
            }

            if (op == OPS.NOT) {
                string exp = expressions.Pop();
                expressions.Push($"!{exp}");
                return;
            }

            if (op >= OPS.EQUALS && op <= OPS.MODULO) {
                if (expressions.Count < 2) {
                    errorWhileParsing = true;
                    errorMessage = $"Parsing error - missing expressions, cannot build the operation {op}";
                    return;
                }
                string exp1 = expressions.Pop();
                string exp2 = expressions.Pop();

                String opSymbol = operatorSymbols[(int)op];
                if (opSymbol.Length == 0) {
                    throw new Exception("Error!");
                }
                expressions.Push($"({exp2}{opSymbol}{exp1})");
                return;
            }

            if (op == OPS.NEGATION) {
                string exp = expressions.Pop();
                expressions.Push($"-{exp}");
                return;
            }

            if (op == OPS.SWIZZLE) {
                string exp = expressions.Pop();
                exp += $".{swizzles[datareader.nextByte()]}";
                expressions.Push($"{exp}");
                return;
            }

            Debug.WriteLine(op);
            if (op == OPS.ENDOFDATA) {
                string final_exp = expressions.Pop();
                dynamicExpressionList.Add(trimb(final_exp));
                Debug.WriteLine("program terminates, should only print ONCE");
            }

        }


        //static string arithmChar(OPS op) {
        //    if (op == OPS.ADD) return "+";
        //    if (op == OPS.SUB) return "-";
        //    if (op == OPS.MUL) return "*";
        //    if (op == OPS.DIV) return "/";
        //    if (op == OPS.MODULO) return "%";
        //    throw new Exception("this cannot happen!");
        //}



        private void processFunction(string funcName, int nrArguments) {
            if (nrArguments == 0) {
                expressions.Push($"{funcName}()");
                return;
            }
            string exp1 = expressions.Pop();
            if (nrArguments == 1) {
                expressions.Push($"{funcName}({trimb(exp1)})");
                return;
            }
            string exp2 = expressions.Pop();
            if (nrArguments == 2) {
                expressions.Push($"{funcName}({exp2},{exp1})");
                return;
            }
            string exp3 = expressions.Pop();
            if (nrArguments == 3) {
                // trim or not to trim ...
                // expressions.Push($"{funcName}({trimb(exp3)},{trimb(exp2)},{trimb(exp1)})");
                expressions.Push($"{funcName}({exp3},{exp2},{exp1})");
                return;
            }

            throw new Exception("this cannot happen!");
        }


        private static string trimb(string exp) {
            return exp[0] == '(' ? exp.Substring(1, exp.Length - 2) : exp;
        }


        private Dictionary<uint, string> externalVariables = new Dictionary<uint, string>();
        private Dictionary<uint, string> localVariables = new Dictionary<uint, string>();

        // naming external variables    EXT, EXT2, EXT3,..
        // using capitals to clearly distinguishing these generated names, or it can get confusing
        private string getExternalVarName(uint varId) {
            externalVariables.TryGetValue(varId, out string varName);
            if (varName == null) {
                if (externalVariables.Count == 0) {
                    varName = "EXT";
                } else {
                    varName = String.Format("EXT{0}", externalVariables.Count);
                }
                externalVariables.Add(varId, varName);
            }
            return varName;
        }


        // naming local variables v1,v2,v3,..
        // the naming of these are not so critical as the external variables because they
        // are fully contained in the texteditor scope (i.e. they won't cause broken links)
        private string getLocalVarName(uint varId) {
            localVariables.TryGetValue(varId, out string varName);
            if (varName == null) {
                varName = String.Format("v{0}", localVariables.Count);
                localVariables.Add(varId, varName);
            }
            return varName;
        }


        private class DataReader {
            private byte[] data;
            public int offset;
            public DataReader(byte[] data) {
                this.data = data;
                this.offset = 0;
            }
            public byte nextByte() {
                return data[offset++];
            }
            public uint nextInt16() {
                uint b0 = nextByte();
                uint b1 = nextByte();
                uint intval = (b1 << 8) + b0;
                return intval;
            }
            public uint nextInt() {
                uint b0 = nextByte();
                uint b1 = nextByte();
                uint b2 = nextByte();
                uint b3 = nextByte();
                uint intval = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
                return intval;
            }
            public float nextFloat() {
                float floatval = BitConverter.ToSingle(data, offset);
                offset += 4;
                return floatval;
            }
            public bool hasData() {
                return offset < data.Length;
            }
        }


        private static string[] swizzles = {
              "x",   "yx",  "zx",  "wx",  "xyx", "yyx", "zyx", "wyx",
              "xzx", "yzx", "zzx", "wzx", "xwx", "ywx", "zwx", "wwx",
              "xxyx","yxyx","zxyx","wxyx","xyyx","yyyx","zyyx","wyyx",
              "xzyx","yzyx","zzyx","wzyx","xwyx","ywyx","zwyx","wwyx",
              "xxzx","yxzx","zxzx","wxzx","xyzx","yyzx","zyzx","wyzx",
              "xzzx","yzzx","zzzx","wzzx","xwzx","ywzx","zwzx","wwzx",
              "xxwx","yxwx","zxwx","wxwx","xywx","yywx","zywx","wywx",
              "xzwx","yzwx","zzwx","wzwx","xwwx","ywwx","zwwx","wwwx",
              "xxxy","yxxy","zxxy","wxxy","xyxy","yyxy","zyxy","wyxy",
              "xzxy","yzxy","zzxy","wzxy","xwxy","ywxy","zwxy","wwxy",
              "xxy", "yxy", "zxy", "wxy", "xy",  "y",   "zy",  "wy",
              "xzy", "yzy", "zzy", "wzy", "xwy", "ywy", "zwy", "wwy",
              "xxzy","yxzy","zxzy","wxzy","xyzy","yyzy","zyzy","wyzy",
              "xzzy","yzzy","zzzy","wzzy","xwzy","ywzy","zwzy","wwzy",
              "xxwy","yxwy","zxwy","wxwy","xywy","yywy","zywy","wywy",
              "xzwy","yzwy","zzwy","wzwy","xwwy","ywwy","zwwy","wwwy",
              "xxxz","yxxz","zxxz","wxxz","xyxz","yyxz","zyxz","wyxz",
              "xzxz","yzxz","zzxz","wzxz","xwxz","ywxz","zwxz","wwxz",
              "xxyz","yxyz","zxyz","wxyz","xyyz","yyyz","zyyz","wyyz",
              "xzyz","yzyz","zzyz","wzyz","xwyz","ywyz","zwyz","wwyz",
              "xxz", "yxz", "zxz", "wxz", "xyz", "yyz", "zyz", "wyz",
              "xz",  "yz",  "z",   "wz",  "xwz", "ywz", "zwz", "wwz",
              "xxwz","yxwz","zxwz","wxwz","xywz","yywz","zywz","wywz",
              "xzwz","yzwz","zzwz","wzwz","xwwz","ywwz","zwwz","wwwz",
              "xxxw","yxxw","zxxw","wxxw","xyxw","yyxw","zyxw","wyxw",
              "xzxw","yzxw","zzxw","wzxw","xwxw","ywxw","zwxw","wwxw",
              "xxyw","yxyw","zxyw","wxyw","xyyw","yyyw","zyyw","wyyw",
              "xzyw","yzyw","zzyw","wzyw","xwyw","ywyw","zwyw","wwyw",
              "xxzw","yxzw","zxzw","wxzw","xyzw","yyzw","zyzw","wyzw",
              "xzzw","yzzw","zzzw","wzzw","xwzw","ywzw","zwzw","wwzw",
              "xxw", "yxw", "zxw", "wxw", "xyw", "yyw", "zyw", "wyw",
              "xzw", "yzw", "zzw", "wzw", "xw",  "yw",  "zw",  "w"};


    }


}

