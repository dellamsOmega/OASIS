namespace NextGenSoftware.OASIS.API.Providers.SOLANAOASIS.Resources;

public static class Messages
{
    private static readonly ResourceManager Resources = new(typeof(Messages).FullName!, typeof(Messages).Assembly);

    public static string SolanaNftTitleCannotBeEmpty => Resources.Get().AsString();
    public static string SolanaNftTitleCannotBeLonger(long bytes) => Resources.Get().Format(bytes);
    public static string SolanaNftSymbolCannotBeEmpty => Resources.Get().AsString();
    public static string SolanaNftSymbolCannotBeLonger(long bytes) => Resources.Get().Format(bytes);

    public static string JsonUrlCannotBeEmpty => Resources.Get().AsString();
    public static string JsonUrlMustBeValid => Resources.Get().AsString();
    public static string JsonUrlCannotBeLonger(long bytes) => Resources.Get().Format(bytes);
    public static string SolanaAccountAddressRequired => Resources.Get().AsString();
    public static string SolanaAccountAddressInvalidFormat => Resources.Get().AsString();
    public static string NftAmountMustBeGreaterThanZero => Resources.Get().AsString();
    public static string NftAmountMustEqualOne => Resources.Get().AsString();

}