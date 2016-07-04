# Yugioh-For-HoloLens
# Test App and Battle Handler

#Battle Handler

UWP apps cannot use regular .dll's (dynamic link libraries). They must use Windows Runtime Components, so we
are putting most (if not all) of the game logic in a runtime component called YuGhiOhBattleHandler.

To use, reference the component by importing the project or finding the compiled .dll, initiate two players,
construct a Game Class and use game.StartGame(). Then players can call functions like NormalSummon() to do
core actions. The game class will keep track of what is played, lifepoints, etc.



#TestApp

To make sure the runtime component works, the test app is just a normal Windows Universal App. When the app is run,
a game layout is displayed:

Top line is Opponents username.
Then Opponents Life Points.
Then Opponents Spell And Trap Zone.
Then Opponents Monster Zone.
Then My Spell and Trap Zone
Then My Monster Zone
Then My Hand (notice cards load and are selectable (turn green)
Then My Username
Then My Lifepoints bar.

So far, you can click on a monster in your hand (it will turn green) and then click on the Monster Zone (light blue)
and it will summon the monster. If you try to summon another, a toast notification will be given saying you already
summoned this turn.