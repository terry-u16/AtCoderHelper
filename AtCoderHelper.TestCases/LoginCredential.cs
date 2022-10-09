using MessagePack;

namespace TerryU16.AtCoderHelper.TestCases;

[MessagePackObject(true)]
public record LoginCredential(string UserName, string Password);
