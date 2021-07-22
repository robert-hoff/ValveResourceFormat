using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestVRF {
	class TryStuff1 {


		static void Mainz() {
			trial1();
		}



		static void trial1() {
			string query = @"SELECT foo, bar
FROM table
WHERE id = 42";


			Debug.WriteLine(query);
		}




	}











}
