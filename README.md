# SFMLCEFLib
A library to easily integrate CEFSharp into SFML.net
## Installation
Download and open the .sln file. Build in either Debug or Release mode (Nuget will fetch packages). Copy the contents from the build or release folder into your project's build folder. Add SFMLCEFLib.dll as a reference.

Note that this currently uses [this](https://github.com/graphnode/SFML.Net) forked version of SFML.net that's more up to date. To use a different version of SFML, remove the SFML nuget reference and add your own (through Nuget or manually with references).
## Usage
```C#
var cefRenderer = new CEFRenderer(window, @"http://msn.com");
while (window.IsOpen)
{
	window.DispatchEvents();
	window.Clear();
	window.Draw(cefRenderer);
	window.Display();
}
```

There is a Sample project included to show the very basics. If VS complains about "CefSharp.Core.dll", please check that you have all the [required files](https://github.com/cefsharp/CefSharp/wiki/Output-files-description-table-(Redistribution)) in the executable directory. If some are missing, go into the /packages folder, and grab the required files.

## Contributing
1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## License
The MIT License (MIT)
Copyright (c) 2016 Dylan Wang

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.