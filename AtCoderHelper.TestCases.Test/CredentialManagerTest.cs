namespace TerryU16.AtCoderHelper.TestCases.Test;

public class CredentialManagerTest
{
    [Fact]
    public async Task SerializeDeserializeTest()
    {
        var manager = new CredentialManager();
        var credential = new LoginCredential("terry_u16", "abc123");

        using var stream = new MemoryStream();
        await manager.SaveCredentialAsync(stream, credential);

        stream.Position = 0;

        var loadedCredential = await manager.LoadCredendialAsync(stream);
        Assert.Equal(credential, loadedCredential);
    }
}