using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace TestVRF
{
    class ParseDynamicExpressions
    {
        // if successful the parsed data assigned here
        public string dynamicExpressionResult = "";
        // parse the input one line at a time, we track certain conditions to delimit these
        public List<string> dynamicExpressionList = new List<string>();

        // if we have problems parsing, no change is made to the input, so fall back
        // on binary data to represent the KV-field
        public bool errorWhileParsing;
        public string errorMessage = "";


        // function reference, name and number of arguments
        private (string, int)[] FUNCTION_REF = {
            ("sin",        1),     // 00
            ("cos",        1),     // 01
            ("tan",        1),     // 02
            ("frac",       1),     // 03
            ("floor",      1),     // 04
            ("ceil",       1),     // 05
            ("saturate",   1),     // 06
            ("clamp",      3),     // 07
            ("lerp",       3),     // 08
            ("dot4",       2),     // 09
            ("dot3",       2),     // 0A
            ("dot2",       2),     // 0B
            ("log",        1),     // 0C
            ("log2",       1),     // 0D
            ("log10",      1),     // 0E
            ("exp",        1),     // 0F
            ("exp2",       1),     // 10
            ("sqrt",       1),     // 11
            ("rsqrt",      1),     // 12
            ("sign",       1),     // 13
            ("abs",        1),     // 14
            ("pow",        2),     // 15
            ("step",       2),     // 16
            ("smoothstep", 3),     // 17
            ("float4",     4),     // 18
            ("float3",     3),     // 19
            ("float2",     2),     // 1A
            ("time",       0),     // 1B
            ("min",        2),     // 1C
            ("max",        2),     // 1D
            ("SrgbLinearToGamma",1), // 1E
            ("SrgbGammaToLinear",1), // 1F
            ("random",     2),     // 20
            ("normalize",  1),     // 21
            ("length",     1),     // 22
            ("sqr",        1),     // 23
            ("TextureSize",1),     // 24
        };


        private static string[] operatorSymbols = {
            "","","","","","","","","","","","","",
            "==","!=",">",">=","<","<=","+","-","*","/","%"};

        private enum OPCODE
        {
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
            NEGATE = 0x18,
            EXTVAR = 0x19,
            SWIZZLE = 0x1E,
            EXISTS = 0x1F,
            // NOT_AN_OPS = 0xff,
        };

        private Stack<string> expressions = new Stack<string>();
        private DataReader datareader;

        // check on each OPS if we are exiting a branch,
        // when we do we should combine expressions on the stack
        private Stack<uint> offsetAtBranchExits = new Stack<uint>();

        public ParseDynamicExpressions(byte[] binary_blob)
        {
            if (externalVarsReference.Count == 0)
            {
                buildExternalVarsReference();
            }

            datareader = new DataReader(binary_blob);
            offsetAtBranchExits.Push(0);

            while (datareader.hasData())
            {
                try
                {
                    ProcessOps((OPCODE)datareader.nextByte());
                } catch (System.ArgumentOutOfRangeException)
                {
                    errorWhileParsing = true;
                    errorMessage = "Parsing error - reader exceeded input";
                    // Debug.WriteLine("EXCEPTION WAS THROWN");
                }
                if (errorWhileParsing)
                {
                    return;
                }
            }

            foreach (string s in dynamicExpressionList)
            {
                dynamicExpressionResult += $"{s}\n";
            }
        }


        private const uint IFELSE_BRANCH = 0;     //    <cond> : <e1> ? <e2>
        private const uint AND_BRANCH = 1;        //    <e1> && <e2>            (these expressions are encoded as branches on the bytestream!)
        private const uint OR_BRANCH = 2;         //    <e1> || <e2>

        private void ProcessOps(OPCODE op)
        {
            // when exiting a branch, combine the conditional expressions on the stack into one
            if (offsetAtBranchExits.Peek() == datareader.offset)
            {
                offsetAtBranchExits.Pop();
                uint branchType = offsetAtBranchExits.Pop();

                switch (branchType)
                {
                    case IFELSE_BRANCH:
                        if (expressions.Count < 3)
                        {
                            errorWhileParsing = true;
                            errorMessage = "error! - not on a branch exit";
                            return;
                        }
                        {
                            string exp3 = expressions.Pop();
                            string exp2 = expressions.Pop();
                            string exp1 = expressions.Pop();
                            // R: it's not safe to trim here
                            // string exp_conditional = $"({trimb2(exp1)} ? {trimb2(exp2)} : {trimb2(exp3)})";
                            string exp_conditional = $"({exp1} ? {exp2} : {exp3})";
                            expressions.Push(exp_conditional);
                        }
                        break;

                    case AND_BRANCH:
                        if (expressions.Count < 2)
                        {
                            errorWhileParsing = true;
                            errorMessage = "parse error, evaluating AND_BRANCH";
                            return;
                        }
                        {
                            string exp2 = expressions.Pop();
                            string exp1 = expressions.Pop();
                            string exp_andcondition = $"({exp1} && {exp2})";
                            expressions.Push(exp_andcondition);
                        }
                        break;

                    case OR_BRANCH:
                        if (expressions.Count < 2)
                        {
                            errorWhileParsing = true;
                            errorMessage = "parse error, evaluating OR_BRANCH";
                            return;
                        }
                        {
                            string exp2 = expressions.Pop();
                            string exp1 = expressions.Pop();
                            string exp_orcondition = $"({exp1} || {exp2})";
                            expressions.Push(exp_orcondition);
                        }
                        break;

                    default:
                        errorWhileParsing = true;
                        errorMessage = "error! this should not happen";
                        return;
                }
            }


            if (op == OPCODE.BRANCH_SEP)
            {
                uint branch_exit = datareader.nextInt16();
                offsetAtBranchExits.Push(branch_exit + 1);
                return;
            }


            // we will need the branch exit, it becomes available when we get to the branch separator
            // (in the middle of the conditional structure)
            if (op == OPCODE.BRANCH)
            {
                uint pointer1 = datareader.nextInt16();
                uint pointer2 = datareader.nextInt16();

                int p = datareader.offset;
                byte[] b = datareader.data;

                // for <e1>&&<e2> expressions we are looking for the pattern
                // 04 12 00 0A 00    07 00 00 00 00
                if (pointer1 - pointer2 == 8 && b[p] == 7 && b[p + 1] == 0 && b[p + 2] == 0 && b[p + 3] == 0 && b[p + 4] == 0)
                {
                    offsetAtBranchExits.Push(AND_BRANCH);
                    datareader.offset += 5;
                    return;
                }

                // for <e1>||<e2> expressions we are looking for the pattern
                // 04 17 00 1F 00     07 00 00 80 3F
                if (pointer2 - pointer1 == 8 && b[p] == 7 && b[p + 1] == 0 && b[p + 2] == 0 && b[p + 3] == 0x80 && b[p + 4] == 0x3F)
                {
                    offsetAtBranchExits.Push(OR_BRANCH);
                    datareader.offset += 5;
                    return;
                }

                offsetAtBranchExits.Push(IFELSE_BRANCH);
                return;
            }


            if (op == OPCODE.FUNC)
            {
                byte funcId = datareader.nextByte();
                byte funcCheckByte = datareader.nextByte();
                if (funcId >= FUNCTION_REF.Length)
                {
                    errorWhileParsing = true;
                    errorMessage = $"Parsing error - invalid function Id = {funcId:x}";
                    return;
                }
                if (funcCheckByte != 0)
                {
                    errorWhileParsing = true;
                    errorMessage = $"Parsing error - malformed data";
                    return;
                }
                string funcName = FUNCTION_REF[funcId].Item1;
                int nrArguments = FUNCTION_REF[funcId].Item2;

                if (nrArguments == -1)
                {
                    errorWhileParsing = true;
                    errorMessage = $"Parsing error - unknown function ID = {funcId:x}";
                    return;
                }
                if (nrArguments > expressions.Count)
                {
                    errorWhileParsing = true;
                    errorMessage = $"Parsing error - too many arguments!";
                    return;
                }

                processFunction(funcName, nrArguments);
                return;
            }


            if (op == OPCODE.FLOAT)
            {
                float float_val = datareader.nextFloat();
                string float_literal = string.Format("{0:g}", float_val);
                // if a float leads with "0." remove the 0 (as how Valve likes it)
                if (float_literal.Length > 1 && float_literal.Substring(0, 2) == "0.")
                {
                    float_literal = float_literal.Substring(1);
                }
                expressions.Push(float_literal);
                return;
            }

            // assignment is always to a local variable, and it terminates the line
            if (op == OPCODE.ASSIGN)
            {
                byte varId = datareader.nextByte();
                string loc_varname = getLocalVarName(varId);
                string exp = expressions.Pop();
                string final_expression = $"{loc_varname} = {trimb(exp)};";
                dynamicExpressionList.Add(final_expression);
                return;
            }

            if (op == OPCODE.LOCALVAR)
            {
                byte varId = datareader.nextByte();
                string loc_varname = getLocalVarName(varId);
                expressions.Push(loc_varname);
                return;
            }

            if (op == OPCODE.NOT)
            {
                string exp = expressions.Pop();
                expressions.Push($"!{exp}");
                return;
            }

            if (op >= OPCODE.EQUALS && op <= OPCODE.MODULO)
            {
                if (expressions.Count < 2)
                {
                    errorWhileParsing = true;
                    errorMessage = $"Parsing error - missing expressions, cannot build the operation {op}";
                    return;
                }
                string exp2 = expressions.Pop();
                string exp1 = expressions.Pop();
                String opSymbol = operatorSymbols[(int)op];
                expressions.Push($"({exp1}{opSymbol}{exp2})");
                return;
            }

            if (op == OPCODE.NEGATE)
            {
                string exp = expressions.Pop();
                expressions.Push($"-{exp}");
                return;
            }

            if (op == OPCODE.EXTVAR)
            {
                uint varId = datareader.nextInt();
                string ext_varname = getExternalVarName(varId);
                expressions.Push(ext_varname);
                return;
            }

            if (op == OPCODE.SWIZZLE)
            {
                string exp = expressions.Pop();
                exp += $".{swizzles[datareader.nextByte()]}";
                expressions.Push($"{exp}");
                return;
            }

            if (op == OPCODE.EXISTS)
            {
                uint varId = datareader.nextInt();
                string ext_varname = getExternalVarName(varId);
                expressions.Push($"exists({ext_varname})");
                return;
            }

            // parser terminates here
            if (op == OPCODE.ENDOFDATA)
            {
                if (datareader.hasData())
                {
                    errorMessage = "malformed data!";
                    errorWhileParsing = true;
                    return;
                }
                string final_exp = expressions.Pop();
                while (final_exp.Length > 2 && final_exp[0] == '(' && final_exp[final_exp.Length - 1] == ')')
                {
                    final_exp = trimb(final_exp);
                }
                dynamicExpressionList.Add($"return {final_exp};");
                return;
            }



            // this point should never be reached
            throw new Exception($"UNKNOWN OPCODE = 0x{(int)op:x2}, offset = {datareader.offset}");

        }


        private void processFunction(string funcName, int nrArguments)
        {
            if (nrArguments == 0)
            {
                expressions.Push($"{funcName}()");
                return;
            }
            string exp1 = expressions.Pop();
            if (nrArguments == 1)
            {
                expressions.Push($"{funcName}({trimb(exp1)})");
                return;
            }
            string exp2 = expressions.Pop();
            if (nrArguments == 2)
            {
                expressions.Push($"{funcName}({trimb(exp2)},{trimb(exp1)})");
                return;
            }
            string exp3 = expressions.Pop();
            if (nrArguments == 3)
            {
                // trim or not to trim ...
                expressions.Push($"{funcName}({trimb(exp3)},{trimb(exp2)},{trimb(exp1)})");
                // expressions.Push($"{funcName}({exp3},{exp2},{exp1})");
                return;
            }
            string exp4 = expressions.Pop();
            if (nrArguments == 4)
            {
                expressions.Push($"{funcName}({trimb(exp4)},{trimb(exp3)},{trimb(exp2)},{trimb(exp1)})");
                return;
            }

            throw new Exception("this cannot happen!");
        }


        private string trimb(string exp)
        {
            return exp[0] == '(' && exp[exp.Length - 1] == ')' ? exp.Substring(1, exp.Length - 2) : exp;
        }
        private string trimb2(string exp)
        {
            return offsetAtBranchExits.Count == 1 ? trimb(exp) : exp;
        }



        private Dictionary<uint, string> externalVariables = new Dictionary<uint, string>();
        private Dictionary<uint, string> localVariables = new Dictionary<uint, string>();

        // naming external variables    EXT, EXT2, EXT3,..
        // using capitals to clearly distinguishing these generated names, or it can get confusing
        private string getExternalVarName(uint varId)
        {
            externalVarsReference.TryGetValue(varId, out string varKnownName);
            if (varKnownName != null)
            {
                return varKnownName;
            }


            externalVariables.TryGetValue(varId, out string varName);
            if (varName == null)
            {
                if (externalVariables.Count == 0)
                {
                    varName = "EXT";
                } else
                {
                    varName = String.Format("EXT{0}", externalVariables.Count + 1);
                }
                externalVariables.Add(varId, varName);
            }
            return varName;
        }


        // naming local variables v1,v2,v3,..
        // the naming of these are not so critical as the external variables because they
        // are contained in the texteditor scope (i.e. they won't cause broken references)
        private string getLocalVarName(uint varId)
        {
            localVariables.TryGetValue(varId, out string varName);
            if (varName == null)
            {
                varName = String.Format("v{0}", localVariables.Count);
                localVariables.Add(varId, varName);
            }
            return varName;
        }


        private static Dictionary<uint, string> externalVarsReference = new Dictionary<uint, string>();
        public static void buildExternalVarsReference()
        {
            // Dota 2
            externalVarsReference.Add(0xd24b982f, "uiTexture");
            externalVarsReference.Add(0x8c954a0d, "fontTexture");
            externalVarsReference.Add(0xbaf50224, "scale1");
            externalVarsReference.Add(0x3b48bcd3, "scale2");
            externalVarsReference.Add(0x7dd532ad, "speed");
            externalVarsReference.Add(0x1ecf71e1, "a");
            externalVarsReference.Add(0xb7fdb72a, "b");
            externalVarsReference.Add(0x1df1849c, "intensity");
            externalVarsReference.Add(0x964485cc, "time");
            externalVarsReference.Add(0x336a0f0c, "$AGE");
            externalVarsReference.Add(0x1527c91c, "$ALPHA");
            externalVarsReference.Add(0xd772913d, "$TRANS_OFFSET_V");
            externalVarsReference.Add(0xa37a3e54, "$TRANS_SCALE_V");
            externalVarsReference.Add(0x25339664, "$OPACITY");
            externalVarsReference.Add(0x69e2f05e, "$TEX_COORD_OFFSET_U");
            externalVarsReference.Add(0x0a5b7f24, "$TEX_COORD_OFFSET_V");
            externalVarsReference.Add(0x7716a69a, "$PA_ARCANA_SPECULAR_BLOOM_SCALE");
            externalVarsReference.Add(0xd73c9c2f, "$PA_ARCANA_DETAIL1BLENDFACTOR");
            externalVarsReference.Add(0x287263fc, "$PA_ARCANA_DETAIL1SCALE");
            externalVarsReference.Add(0xd4147a1f, "$PA_ARCANA_DETAIL1TINT");
            externalVarsReference.Add(0xa58452dc, "$PA_ARCANA_SPECULAR_BLOOM_COLOR");
            externalVarsReference.Add(0x514616e6, "$GemColor");
            externalVarsReference.Add(0x9eac976a, "$overbright");
            externalVarsReference.Add(0xab2163a4, "$TEX_COLOR");
            externalVarsReference.Add(0x3225af29, "y");
            externalVarsReference.Add(0x84321e5f, "$DETAILBLEND");
            externalVarsReference.Add(0x276085fb, "FadeOut");
            externalVarsReference.Add(0xc4a1f8f7, "$COLOR");
            externalVarsReference.Add(0xf588a3d3, "$CLOAKINT");
            externalVarsReference.Add(0xda4e0212, "$SPIN");
            externalVarsReference.Add(0xb57746a1, "panoramaTexCoordOffset");
            externalVarsReference.Add(0xe244f4af, "panoramaTexCoordScale");
            externalVarsReference.Add(0x341f4361, "panoramaLayer");
            externalVarsReference.Add(0x1b927481, "avatarTexture");
            externalVarsReference.Add(0x57b2b714, "$ent_Health");
            externalVarsReference.Add(0x546a87df, "proceduralSprayTexture");
            externalVarsReference.Add(0x9d389d79, "alive");
            externalVarsReference.Add(0x46ec689a, "zz");
            externalVarsReference.Add(0x7068cf59, "aa");
            externalVarsReference.Add(0xff492a3a, "bb");
            externalVarsReference.Add(0xcb9a78d4, "cc");
            externalVarsReference.Add(0xde117c4a, "dd");
            externalVarsReference.Add(0xeb075669, "ee");
            externalVarsReference.Add(0x285fc55e, "ff");
            externalVarsReference.Add(0x39de2fbd, "FadeOut_blade");

            // HL Alyx
            externalVarsReference.Add(0xe7dc4bd6, "$BaseTexture");
            externalVarsReference.Add(0x30ee22ba, "colorAttrMovie");
            externalVarsReference.Add(0x98a42c96, "$DISSOLVE");
            externalVarsReference.Add(0x13e5d925, "$BRIGHTNESS");
            externalVarsReference.Add(0x097ad797, "logo_draw");
            externalVarsReference.Add(0xbad34216, "$SELFILLUM");
            externalVarsReference.Add(0xf8d95bff, "$SCROLLX");
            externalVarsReference.Add(0xfd912b88, "$SCROLLY");
            externalVarsReference.Add(0x0b2a3d85, "$EMISSIVEBRIGHTNESS");
            externalVarsReference.Add(0x41b948dc, "$EMISSIVESCALE");
            externalVarsReference.Add(0x8f3e65c3, "$NOISE");
            externalVarsReference.Add(0xd4db18d9, "$Emissive");
            externalVarsReference.Add(0x2797e0f8, "$Time");
            externalVarsReference.Add(0xa359c3d2, "$Enabled");
            externalVarsReference.Add(0x0a7ef0bc, "colorAttr");
            externalVarsReference.Add(0x52d9cda7, "$SPEED");
            externalVarsReference.Add(0x26b36985, "$FRESNEL");
            externalVarsReference.Add(0xd7d9c882, "$LINEWIDTH");
            externalVarsReference.Add(0x09e85963, "$COLOR2");
            externalVarsReference.Add(0xb4f6068c, "$EMISSIVE_COLOR");
            externalVarsReference.Add(0x9c865576, "$EyeBrightness");
            externalVarsReference.Add(0x626b58e4, "$TRANS");
            externalVarsReference.Add(0x99ed5df3, "gmanEyeGlow");
            externalVarsReference.Add(0xcba2f3ed, "$jawOpen");
            externalVarsReference.Add(0xe1ea5a51, "$ILLUMDEATH");
            externalVarsReference.Add(0xac2455ce, "$EmSpeed");
            externalVarsReference.Add(0x354cd34e, "useglow");
            externalVarsReference.Add(0xc2a33a98, "$IconCoordOffset");
            externalVarsReference.Add(0x64001e52, "$IconCoordScale");
            externalVarsReference.Add(0xfb2f9805, "$CounterIcon");
            externalVarsReference.Add(0x256a1960, "$CounterDigitHundreds");
            externalVarsReference.Add(0x4373b9f9, "$CounterDigitTens");
            externalVarsReference.Add(0x90c26f54, "$CounterDigitOnes");
            externalVarsReference.Add(0xbaeebc0b, "$HealthLights");
            externalVarsReference.Add(0xdea79565, "$FrameNumber1");
            externalVarsReference.Add(0x66a9e338, "$FrameNumber2");
            externalVarsReference.Add(0xcd41b4b8, "$FrameNumber3");
            externalVarsReference.Add(0xe4200216, "origin");
            externalVarsReference.Add(0x9550cca8, "value1");
            externalVarsReference.Add(0x7f787303, "$POSITION");
            externalVarsReference.Add(0xcb9c152d, "advisorMovie");
            externalVarsReference.Add(0xa55cfdd3, "$ANIM");
            externalVarsReference.Add(0xfac4270a, "$LightValue");
            externalVarsReference.Add(0xb5e34aab, "$PercentAwake");
            externalVarsReference.Add(0x435a062f, "$ENERGY");
            externalVarsReference.Add(0xe813cc7e, "$FLOW");
            externalVarsReference.Add(0x38b70d43, "$SCALE");
            externalVarsReference.Add(0x5a8f66c4, "$COLORA");
            externalVarsReference.Add(0x0f09ee7b, "$AnimatePipes");
            externalVarsReference.Add(0xbf319cc2, "$IlluminatePipes");
            externalVarsReference.Add(0x8715f68f, "$PistolChamberReadout");
            externalVarsReference.Add(0x98e238a9, "$PistolClipReadoutOffset");
            externalVarsReference.Add(0x90259463, "$PistolClipReadoutScale");
            externalVarsReference.Add(0xa58adf84, "$PistolHopperReadout");
            externalVarsReference.Add(0xc803a08e, "$InjectedPercent");
            externalVarsReference.Add(0x3666c43e, "$FrameNumber");
            externalVarsReference.Add(0x65f09c96, "$GrenadeLEDBrightness");
            externalVarsReference.Add(0x84e355f4, "$GrenadeLEDFuse");
            externalVarsReference.Add(0xc858079b, "$CableBrightness");
            externalVarsReference.Add(0x507c31f7, "$ChamberBrightness");
            externalVarsReference.Add(0x4f60e501, "$EnergyBallCharged");
            externalVarsReference.Add(0x47940a69, "$ReadyToExplode");
            externalVarsReference.Add(0x3cf1f4a5, "$AmmoColor");
            externalVarsReference.Add(0xefe71421, "$BulletCount");
            externalVarsReference.Add(0x79530848, "$MaxBulletCount");
            externalVarsReference.Add(0x71ee8c47, "$SlideLight");
            externalVarsReference.Add(0xe399c3c7, "$ShotgunHandleLight");
            externalVarsReference.Add(0x5260e007, "$QuickFireLight");
            externalVarsReference.Add(0x72711be3, "$LaserEmitterBrightness");
            externalVarsReference.Add(0x386b35f0, "$LaserEmitterFlowSpeed");
            externalVarsReference.Add(0x73119842, "$LightColor");
        }


        private class DataReader
        {
            public byte[] data;
            public int offset;
            public DataReader(byte[] data)
            {
                this.data = data;
                this.offset = 0;
            }
            public byte nextByte()
            {
                return data[offset++];
            }
            public uint nextInt16()
            {
                uint b0 = nextByte();
                uint b1 = nextByte();
                uint intval = (b1 << 8) + b0;
                return intval;
            }
            public uint nextInt()
            {
                uint b0 = nextByte();
                uint b1 = nextByte();
                uint b2 = nextByte();
                uint b3 = nextByte();
                uint intval = (b3 << 24) + (b2 << 16) + (b1 << 8) + b0;
                return intval;
            }
            public float nextFloat()
            {
                float floatval = BitConverter.ToSingle(data, offset);
                offset += 4;
                return floatval;
            }
            public bool hasData()
            {
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




