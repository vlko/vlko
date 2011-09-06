
namespace vlko.BlogModule.NH.Tests.Repository.NRepository.Implementation
{
	public enum TypeEnum
	{
		SomeFirstType,
		SomeOtherType,
		Ignore
	}
	public class NTestObject
	{
		public int ID { get; set; }

		public string Text { get; set; }

		public TypeEnum Type { get; set; }

		public override bool Equals(object obj)
		{
			if ((obj != null) && (obj is NTestObject))
			{
				var compare = obj as NTestObject;
				if (compare.ID.Equals(ID)
					&& (compare.Text.Equals(Text))
					&& (compare.Type.Equals(Type)))
				{
					return true;
				}
			}
			return false;
		}
	}
}
