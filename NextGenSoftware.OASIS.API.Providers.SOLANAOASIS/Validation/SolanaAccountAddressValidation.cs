namespace NextGenSoftware.OASIS.API.Providers.SOLANAOASIS.Validation;

public sealed class SolanaAccountAddressValidation : AbstractValidator<string>
{
    public static readonly SolanaAccountAddressValidation Instance = new();

    private SolanaAccountAddressValidation()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage(Messages.SolanaAccountAddressRequired)
            .Must(IsValidSolanaAddress).WithMessage(Messages.SolanaAccountAddressInvalidFormat);
    }

    private bool IsValidSolanaAddress(string address)
    {
        try
        {
            PublicKey pubKey = new(address);
            return pubKey.KeyBytes.Length == 32;
        }
        catch
        {
            return false;
        }
    }
}