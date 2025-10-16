using BotForge.Messaging;
using Xunit;

namespace BotForge.Tests.Messaging;

public class LabelStoreReflectionTests
{
    [Fact]
    public void LabelStore_DoesNotThrow_OnConstructionAndLookup()
    {
        // Arrange & Act
        var store = new LabelStore();

        // Request a key that may not exist — method should return a ButtonLabel (or equivalent) and not throw
        var label = store.GetLabel("__NonExistingTestKey__");

        // Assert
        Assert.NotNull(label);
    }
}
