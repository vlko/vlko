namespace vlko.core.Testing
{
	public abstract class LocalTest
	{
		public ITestProvider TestProvider { get; private set; }

		public LocalTest(ITestProvider testProvider)
		{
			TestProvider = testProvider;
		}
	}
}
