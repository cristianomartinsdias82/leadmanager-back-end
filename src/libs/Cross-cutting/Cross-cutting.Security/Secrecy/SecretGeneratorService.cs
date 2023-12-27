namespace CrossCutting.Security.Secrecy;

internal sealed class SecretGeneratorService : ISecretGenerationService
{
    public async ValueTask<string> GenerateAsync(
        int minLength,
        int maxLength,
        bool includeLowercaseLetters = true,
        bool includeUpperCaseLetters = true,
        bool includeNumbers = true,
        bool includeSpecialCharacters = true,
        CancellationToken cancellationToken = default)
        => await ValueTask.FromResult(Random.Shared.Next(100000, 999999).ToString());
}
