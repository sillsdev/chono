### What is this repository for? ###

* Quotelighter highlights quote levels and helps Bible translation teams apply Glyssen biblical character names to quotes.

### How do I get set up? ###

* To get set up to use and build Quotelighter, you will need to download [Paratext](https://paratext.org/).
* There are some post-build commands that will attempt to copy the plugin files to a (potentially) useful location if you are building using the "Debug - Copy to Paratext" or "Release - Copy to Paratext" configurations. Depending on your individual needs, you might want to tweak the details, but if you do, please don't include your tweaks in a pull request. When building the "Release - Copy to Paratext" configuration for the first time, it will attempt to set up the necessary directory structure by copying files into the default install location(s) for Paratext. You need to be running Visual Studio as an administrator, or the robocopy command(s) will fail.
* To learn to use Quotelighter, see the [wiki](https://github.com/ubsicap/paratext_demo_plugins/wiki).
* Unit tests depend on [NUnit](https://nunit.org/). I recommend using [Jet Brains Resharper](https://www.jetbrains.com/resharper/), which has built-in test running capabilities.
* The [Paratext Demo Plugins](https://github.com/ubsicap/paratext_demo_plugins) repository has more advanced information about the Paratext plugin architecture, which will explain more about how to build a plugin like Quotelighter.

### Contribution guidelines ###

* Contributions that further the goals of this project are welcome.
* If your pull request does not contain passing unit tests that demonstrate the value and quality of your contribution, it will reduce the likelihood that your submission will be accepted.
* If you contact me in advance of making any changes, I can probably give you some guidance and let you know if I think your proposed changes are on track.

### Who do I talk to? ###

* Technical lead: [Tom Bogle](mailto:tom_bogle@sil.org)