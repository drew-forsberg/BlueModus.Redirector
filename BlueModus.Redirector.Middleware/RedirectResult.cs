namespace BlueModus.Redirector.Middleware
{
    public class RedirectResult
    {
        public required string TargetUrl { get; set; }
        public bool IsPermanent { get; set; }
    }
}