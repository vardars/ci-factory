using NAnt.Core.Attributes;

namespace NAnt.Core.Tasks
{
	[TaskName("break")]
	public class BreakTask : Task
	{

		private static bool _Break;

		public static bool Break
		{
			get
			{
				return _Break;
			}
			set
			{
				_Break = value;
			}
		}

		protected override void ExecuteTask()
		{
			Break = true;
		}
	}
}