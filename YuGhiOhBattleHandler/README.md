# Yugioh-For-HoloLens
# Test App and Battle Handler

#Battle Handler

UWP apps cannot use regular .dll's (dynamic link libraries). They must use Windows Runtime Components, so we
are putting most (if not all) of the game logic in a runtime component called YuGhiOhBattleHandler.

To use, reference the component by importing the project or finding the compiled .dll, initiate two players,
construct a Game Class and use game.StartGame(). Then players can call functions like NormalSummon() to do
core actions. The game class will keep track of what is played, lifepoints, etc.

This is complete for the first 40 cards. We are working on making this into a PCL, too.



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

All battling should be present. Please note any bugs. Click on a monster in your hand (it will highlight green). If
it is a monster, you can then click on the monster zone to summon it (1 per turn). Then end your turn with the end
turn button. The opponent will summon a monster and it will be your turn again. Double click on a monster to change
it from attack to defense mode. Click on a monster and then on an opponents monster to attack it. Use spells by
clicking on your hand and then clicking on the spell and trap zone.