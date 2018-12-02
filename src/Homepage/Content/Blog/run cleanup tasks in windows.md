# Run Cleanup Tasks in Windows

This is just some quick note to not forget it

Create a batchfile called clean.cmd on your desktop. Fill it with the following content:

```
Rundll32.exe advapi32.dll,ProcessIdleTasks
defrag -b %SystemDrive%
```

Run that batchfile as admin. Then wait. It’ll take a while.

I typically use this on non-ssd based machines of friends after a clean setup and all the updates. First, I run disk cleaner to cleanup all old updates/upgrades (for machines that i upgrade to win10, for example), then as a last task, i run this command.