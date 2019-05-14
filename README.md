# ManualTester
ManualTester is my private project. It's Windows Forms Application, written in C#.  
✭ It executes actions on an android device that is plugged in to the computer  
✭ It reports the result based on logcat output  

This app allows to execute repetitive sequences of actions, which can be used for testing applications.  
✭ Can be used with any app, even with the release version of product  
✭ Doesn't consume device's resources (CPU/RAM) which is very helpful when you're looking for race condition bugs or bugs requiring specific timing.  
✭ Checks if action execution was success or failure, or an error occured* and is able to point out the exact part that couldn't be executed so it's more than just a bot tapping around.


*looking for errors (blacklisted logs) is a TODO  

**The project still has major TODOs (it's barely MVP):**   
[ ] Code Review feedback implementation  
[ ] Blacklisted logs (test fails after specific log appeared, for example an exception)  
[ ] Support for different screen resolutions (now works only for 1080 x 1920)  
[ ] Support for different aspect ratios (now works only for 16:9)  
[ ] Various types of actions (now, only a tap is supported)  
[ ] Cancelling tests  
[ ] Test Step Editor  
All TODOs are listed on my trello: https://trello.com/invite/b/oEHOagYS/4faaf133e6399e3d030b015f113e2450/manualtester  

**How to use**  
!! You will need android platform-tools and adb added to PATH environmental variable.   

1. Copy the "example" folder. Rename "example" to your apps package name.  
2. Open "TestStepDefinitions.json" file in "packagename/Definitions". You will need to create similar definitions for the app you want to test.  
3. After defining the single "steps", you need to put them up in a sequence - take a look at the "TestSequenceDefinitions" file in packagename/Definitions.  
4. Launch Manual Tester and choose the application you want to test (note: choosing another application is still not implemented - it's a TODO).   
5. Sequences names will appear in the left box. Please select the sequences you'd like to be executed by placing them in the right box.  
6. Now switch to the second tab and click "RUN TEST".  
7. Choosen sequence will be executed (you can see list of steps in the box below - steps that ended successfully will be marked with a ✓)  
