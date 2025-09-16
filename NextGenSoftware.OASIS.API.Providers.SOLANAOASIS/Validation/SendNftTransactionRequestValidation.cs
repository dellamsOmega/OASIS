namespace NextGenSoftware.OASIS.API.Providers.SOLANAOASIS.Validation;

public sealed class SendNftTransactionRequestValidation : AbstractValidator<NFTWalletTransactionRequest>
{
    public static readonly SendNftTransactionRequestValidation Instance = new();

    private SendNftTransactionRequestValidation()
    {
        RuleFor(x => x.FromWalletAddress)
            .NotEmpty().WithMessage(Messages.SolanaAccountAddressRequired)
            .Must(IsValidSolanaAddress).WithMessage(Messages.SolanaAccountAddressInvalidFormat);

        RuleFor(x => x.ToWalletAddress)
            .NotEmpty().WithMessage(Messages.SolanaAccountAddressRequired)
            .Must(IsValidSolanaAddress).WithMessage(Messages.SolanaAccountAddressInvalidFormat);

        RuleFor(x => x.TokenAddress)
            .NotEmpty().WithMessage(Messages.SolanaAccountAddressRequired)
            .Must(IsValidSolanaAddress).WithMessage(Messages.SolanaAccountAddressInvalidFormat);

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage(Messages.NftAmountMustBeGreaterThanZero)
            .Must(amount => amount == 1)
            .WithMessage(Messages.NftAmountMustEqualOne);
    }

    private bool IsValidSolanaAddress(string address)
    {
        try
        {
            var pubKey = new PublicKey(address);
            return pubKey.KeyBytes.Length == 32;
        }
        catch
        {
            return false;
        }
    }
}