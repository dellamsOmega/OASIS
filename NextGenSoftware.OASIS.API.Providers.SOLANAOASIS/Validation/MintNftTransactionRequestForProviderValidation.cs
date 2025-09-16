namespace NextGenSoftware.OASIS.API.Providers.SOLANAOASIS.Validation;

public sealed class MintNftTransactionRequestForProviderValidation : AbstractValidator<MintNFTTransactionRequestForProvider>
{
    public static readonly MintNftTransactionRequestForProviderValidation Instance = new();

    private const int MaxNameBytes = 32;
    private const int MaxSymbolBytes = 10;
    private const int MaxUriBytes = 200;

    private MintNftTransactionRequestForProviderValidation()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(Messages.SolanaNftTitleCannotBeEmpty)
            .Must(BeWithinNameByteLimit).WithMessage(Messages.SolanaNftTitleCannotBeLonger(MaxNameBytes));

        RuleFor(x => x.Symbol)
            .NotEmpty().WithMessage(Messages.SolanaNftSymbolCannotBeEmpty)
            .Must(BeWithinSymbolByteLimit).WithMessage(Messages.SolanaNftSymbolCannotBeLonger(MaxSymbolBytes));

        RuleFor(x => x.JSONUrl)
            .NotEmpty().WithMessage(Messages.JsonUrlCannotBeEmpty)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage(Messages.JsonUrlMustBeValid)
            .Must(BeWithinUriByteLimit).WithMessage(Messages.JsonUrlCannotBeLonger(MaxUriBytes));
    }

    private bool BeWithinNameByteLimit(string title) => Encoding.UTF8.GetByteCount(title ?? "") <= MaxNameBytes;
    private bool BeWithinSymbolByteLimit(string symbol) => Encoding.UTF8.GetByteCount(symbol ?? "") <= MaxSymbolBytes;
    private bool BeWithinUriByteLimit(string uri) => Encoding.UTF8.GetByteCount(uri ?? "") <= MaxUriBytes;
}