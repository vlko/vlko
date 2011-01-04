namespace vlko.BlogModule.Action.ComplexHelpers.Twitter
{
	
	public class OAuthToken : ConsumerAppIdent
	{
		public virtual string Token { get; set; }
		public virtual string TokenSecret { get; set; }
	}
}
