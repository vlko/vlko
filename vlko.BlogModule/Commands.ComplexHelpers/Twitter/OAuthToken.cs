namespace vlko.BlogModule.Commands.ComplexHelpers.Twitter
{
	
	public class OAuthToken : ConsumerAppIdent
	{
		public virtual string Token { get; set; }
		public virtual string TokenSecret { get; set; }
	}
}
