using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonistHistory {
	public class ColonistHistoryDef : Def {
		public Type workerClass = typeof(ColonistHistoryWorker);

		private ColonistHistoryWorker workerInt;

		public bool defaultOutput = true;

		public Type valueType;
		public Type defType;

		public ColonistHistoryWorker Worker {
			get {
				if (this.workerInt == null && this.workerClass != null) {
					this.workerInt = (ColonistHistoryWorker)Activator.CreateInstance(this.workerClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}

		public override IEnumerable<string> ConfigErrors() {
			foreach (string text in base.ConfigErrors()) {
				yield return text;
			}
			if (this.valueType == null) {
				yield return "no valueType";
			}
		}
	}
}
