using LudumDare54.Core.States;

namespace LudumDare54.Core.Tests;

[TestClass]
public class SessionTests {
    [TestMethod]
    public async Task SessionRunWithoutConditions() {
        var repository = new Repository();
        repository.ResourceCards.Add(new ResourceCard("Test", false));
        repository.EventCards.Add(new EventCard("Test", new List<Outcome>() {
            new() {
                Id = "default",
                Weight = 1.0f,
                Next = new List<NextEventCard>() {
                    new("Test", 1.0f)
                }
            }
        }));

        var stateManager = new StateManager();

        var session = new Session();

        Int32 deckSelection = 0;
        Int32 cardSelection = 0;
        Int32 cardResult = 0;

        stateManager.InputCallback = state => {
            if (state is DeckSelectionState deckSelectionState) {
                deckSelection ++;
                for(var i = 0; i < deckSelectionState.Amount; i++) {
                    deckSelectionState.Deck.Add(repository.ResourceCards[0]);
                }
                deckSelectionState.Finish();
            }
            else if (state is CardSelectionState cardSelectionState) {
                cardSelection++;
                cardSelectionState.Finish(cardSelectionState.Deck[0]);
            }
            else if (state is CardResultState cardResultState) {
                cardResult++;
                if (cardResult == 2) {
                    session.Active = false;
                }
                cardResultState.Finish();
            }
        };

        await session.Play(repository, stateManager);

        Assert.AreEqual(1, deckSelection);
        Assert.AreEqual(2, cardSelection);
        Assert.AreEqual(2, cardResult);
    }
}