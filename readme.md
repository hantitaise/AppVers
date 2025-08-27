# AppVers
Powershell tools to simulate npm version, with simple script PS

# Reason
I use ussually npm version command, to update package.json when use develop with node js.
And I like put in my webapps, in title or in client UI render, to be sure that align to the latest version. 
Sometimes, browser have Cache or users didn't clode/reopen or refresh there page
It's very usefull

But when I developp in Python, PHP ... I didn't have this tools except if install node or some other packages.

So I developp this tools

Usage: .\version options
Version Init => create file version.json is not exsit and ask basic questions.
Version major => Increase major version ex: 1.0.0 -> 2.0.0
Version minor => Increase major version ex: 1.1.0 -> 1.2.0
Version patch => Increase major version ex: 1.1.1 -> 1.1.2

So simple :stuck_out_tongue_winking_eye:

# version sources
- C#
- PHP
- Python
- Rust
- Powershell (Primary Implementation)