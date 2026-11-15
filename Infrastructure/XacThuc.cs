namespace QuanLyHanMucWeb.Infrastructure
{
    public class CauHinhXacThuc
    {
        public string CheDo { get; set; } = "Local";
        public CauHinhOidcThamSo Oidc { get; set; } = new CauHinhOidcThamSo();
        public CauHinhKeycloak Keycloak { get; set; } = new CauHinhKeycloak();

        public bool DungOidc => string.Equals(CheDo, "Oidc", StringComparison.OrdinalIgnoreCase);
    }

    public class CauHinhOidcThamSo
    {
        public string Authority { get; set; }
        public string MetadataAddress { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }

    public class CauHinhKeycloak
    {
        public string AdminUrl { get; set; }
        public string Realm { get; set; } = "hanmuc";
        public string AdminUser { get; set; }
        public string AdminPassword { get; set; }
    }

    public static class TenScheme
    {
        public const string Oidc = "oidc";
    }
}
