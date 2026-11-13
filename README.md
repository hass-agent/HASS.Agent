[![GitHub release (latest by date)](https://img.shields.io/github/v/release/hass-agent/HASS.Agent)](https://github.com/hass-agent/HASS.Agent/releases/)
[![license](https://img.shields.io/badge/license-MIT-blue)](#license)
[![OS - Windows](https://img.shields.io/badge/OS-Windows-blue?logo=windows&logoColor=white)](https://www.microsoft.com/ "Go to Microsoft homepage")
[![dotnet](https://img.shields.io/badge/.NET-8.0-blue)](https://img.shields.io/badge/.NET-8.0-blue)
![GitHub all releases](https://img.shields.io/github/downloads/hass-agent/HASS.Agent/total?color=blue)
![GitHub latest](https://img.shields.io/github/downloads/hass-agent/HASS.Agent/latest/total?color=blue)
[![Discord](https://img.shields.io/badge/dynamic/json?color=blue&label=Discord&logo=discord&logoColor=white&query=presence_count&suffix=%20Online&url=https://discord.com/api/guilds/1173033284519862392/widget.json)](https://discord.com/invite/JfZj98xqJr)

<a href="https://github.com/hass-agent/HASS.Agent/">
    <img src="https://raw.githubusercontent.com/LAB02-Research/HASS.Agent/main/images/logo_128.png" alt="HASS.Agent logo" title="HASS.Agent" align="right" height="128" />
</a>

# HASS.Agent

HASS.Agent is a Windows-based client (*companion*) application for [Home Assistant](https://www.home-assistant.io), developed in .NET 8.

Click [here](https://github.com/hass-agent/HASS.Agent/releases/latest/download/HASS.Agent.Installer.exe) to download the latest installer.

----

HASS.Agent is **completely free**, and will always stay that way without restrictions. 

----

### Contents

 * [Why?](#why)
 * [Fork?](#fork)
 * [Functionality](#functionality)
 * [Screenshots](#screenshots)
 * [Installation](#installation)
 * [Help and Documentation](#help-and-documentation)
 * [Articles](#articles)
 * [What it's not](#what-its-not)
 * [Helping Out](#helping-out)
 * [Credits and Licensing](#credits-and-licensing)
 * [Legacy](#legacy)

----

### Why?

Quick note from Sam on the initial idea:
> The main reason I built this is that I wanted to receive notifications on my PC, including images, and to quickly perform actions (e.g. to toggle a lamp). There weren't any software-based solutions for this, so I set out to build one myself. 

There's no need to explain that we (and the Community overall) like the idea. That's why we're here, to continue the development in the Home Assistant spirit of integrating **everything** into open source smarthome world.

----

### Fork?

The original HASS.Agent has been created by [Sam](https://github.com/LAB02-Admin).

Unfortunately due to some time constraints, they're not able to provide the constant support and feature updates. That's where we step in - trying to keep HASS.Agent bug free (dreams need to be big right?) and to introduce new features here and there!

#### Note on the organization and project name
*"Why this project is named the same as original HASS.Agent, it's confusing"*

Yes, I agree, you're right. The initial idea after Sam's disappearance was to continue the work and offer it to Sam once they're back.
Well, now that's more than unlikely since it's been quite a long time we last spoke with Sam.

Knowing what we do now, we've probably made a different decision but doing it now it'll only create more confusion. We have a full rewrite project going and it has been already decided the name is going to be altered.

----

### Functionality

Summary of the core functions:

* **Notifications**: receive notifications, show them using Windows builtin toast popups, attach images and receive input from them. Supports *actionable notifications*: add buttons so you can easily interact with Home Assistant, without having to open anything or ask user for an answer to a question.
  - *This requires the installation of the [HASS.Agent integration](https://github.com/hass-agent/HASS.Agent-Integration)*.

* **Media Player**: use HASS.Agent as a mediaplayer device: see and control what's playing and send text-to-speech.
  - *This requires the installation of the [HASS.Agent integration](https://github.com/hass-agent/HASS.Agent-Integration)*.

* **Quick Actions**: use a keyboard shortcut to quickly pull up a command interface, through which you can control Home Assistant entities - or, assign a keyboard shortcut to individual Quick Actions for even faster triggering.

* **Commands**: control your PC (or other Windows based device) through Home Assistant using custom- or built-in commands.

* **Sensors**: send your PC's sensors to Home Assistant to monitor every aspect of your device.

* **WebView**: quickly show any website, anywhere - no browser required, for instance a HA dashboard.

* **Satellite Service**: use the service to collect sensor data and execute commands, even when you're not logged in (**not all commands/sensors are available for Satellite Service**)

* All entities are dynamically acquired from your Home Assistant instance.

* Commands and sensors are automatically added to your Home Assistant instance via MQTT Integration

----

### Screenshots

Notification examples:




![image](https://user-images.githubusercontent.com/81011038/199956334-642def7d-4cb4-46f3-a73b-25c76e5bd02c.png)
![261428315-fa66e0cf-bd41-49d6-956c-864eec4bcc70](https://github.com/amadeo-alex/HASS.Agent/assets/68441479/c7e35fb1-59ea-4077-983e-916735dd3901)
![Text-based toast notification](https://raw.githubusercontent.com/LAB02-Research/HASS.Agent/main/images/hass_agent_toast_text.png)


WebView example, showing a dashboard when right-clicking the tray icon:

![WebView](https://user-images.githubusercontent.com/81011038/174068053-971adb8b-f552-43bc-a39b-6c6c4a3bda9c.png)

This is the Quick Action window you'll see when using the hotkey. This window automatically resizes to the amount of buttons you've added:

![Quick Actions](https://raw.githubusercontent.com/LAB02-Research/HASS.Agent/main/images/hass_agent_quickactions.png)

You can easily configure a new Quick Action, HASS.Agent will fetch your entities for you:

![New Quick Actions](https://raw.githubusercontent.com/LAB02-Research/HASS.Agent/main/images/hass_agent_new_quickaction.png)

The sensors configuration screen:

![Sensors](https://raw.githubusercontent.com/LAB02-Research/HASS.Agent/main/images/hass_agent_sensors.png)
    
Adding a new sensor is just as easy:

![Sensors](https://raw.githubusercontent.com/LAB02-Research/HASS.Agent/main/images/hass_agent_new_sensor.png)

Easily manage the satellite service through HASS.Agent:

![Service](https://raw.githubusercontent.com/LAB02-Research/HASS.Agent/main/images/hass_agent_satellite_service.png)

You'll be guided through the configuration options during onboarding:

![Onboarding](https://user-images.githubusercontent.com/81011038/198251220-d15b4b3b-264e-44bc-b52f-5c404f9efb1f.png)
    
----

### Installation

Installing HASS.Agent is easy; just [download the latest installer](https://github.com/hass-agent/HASS.Agent/releases/latest/download/HASS.Agent.Installer.exe), run it and you're done! The installer is signed by us and won't download or do weird stuff - it just places everything where it should, and launches with the right parameter. (optionally installing .NET8)

After installing, the onboarding process will help you get everything configured, step by step. If you want an introduction into HASS.Agent, be sure to read the [introduction docs](https://www.hass-agent.io/latest/getting-started/).

Original HASS.Agent documentation is available [here](https://www.hass-agent.io/latest/getting-started/#introduction) - please bear in mind however that it may not represent state of things present in this version.

[Click here to download the latest installer](https://github.com/hass-agent/HASS.Agent/releases/latest/download/HASS.Agent.Installer.exe)

If you want to install manually, there are .zip packages available for every release. Read the [manual](https://www.hass-agent.io/latest/getting-started/installation/#manualzip-files) for more info.

----

### Help and Documentation

Stuck while installing or using HASS.Agent, need some help integrating the sensors/commands or have a great idea for the next version? There are a few channels through which you can reach out:

* [Github Tickets](https://github.com/hass-agent/HASS.Agent/issues): Report bugs, feature requests, ideas, tips, ..

* [Documentation](https://www.hass-agent.io/latest/): Installation, configuration and usage documentation, as well as examples.

* [Discord](https://discord.com/invite/JfZj98xqJr): Get help with setting up and using HASS.Agent, report bugs or just talk about whatever.

* [Home Assistant forum](https://community.home-assistant.io/): Bit of everything, with the addition that other HA users can help as well.

Starting from zero, and want to learn what HASS.Agent's about and how to start? Be sure to check the [introduction article](https://www.hass-agent.io/latest/getting-started/#introduction), and optionally the [command basics](https://hassagent.readthedocs.io/en/latest/commands/command-basics/).

EverythingSmartHome's youtube video is a great guide on the original HASS.Agent version: [Control Your Windows PC With Home Assistant!](https://www.youtube.com/watch?v=B4SnJPVbSXc). We recommend having a look at his other videos as well, great stuff!

If you want to help with the development of HASS.Agent, check out the [Helping Out](#helping-out) section for (translating) info.

----

### Articles

### Original HASS.Agent by Sam/LAB02 Research

Liam Alexander Colman from [Home Assistant Guide](https://home-assistant-guide.com) was kind enough to write an article about HASS.Agent: [Integrate Home Assistant with Windows using HASS.Agent](https://home-assistant-guide.com/2022/04/20/integrate-home-assistant-with-windows-using-hass-agent/). The website's full of useful articles, worth having a look :)

----

### What it's not

A Linux/macOS client (at least yet)! 

Without getting into much of the details, it's not as easy as you think.

With HASS.Agent "2.X" version it's basically impossible. We are thinking about cross-platform support for the "v3 rewrite" but as of now it's only a hopeful wish.

You can try the [official companion app](https://apps.apple.com/us/app/home-assistant/id1099568401) for macOS, or [IoPC](https://github.com/maksimkurb/IoPC) which runs on Linux.

#### Note: We haven't tested either and we do not track the development efforts.

----

### Helping Out

The best way to help out is to test as much as you can (or even join the beta program), and report any weird or failing behavior by [opening a ticket](https://github.com/hass-agent/HASS.Agent/issues). 

Same goes for sharing ideas for new (or improved) functionality! If you want, you can [join on Discord](https://discord.com/invite/JfZj98xqJr) to discuss your ideas.

#### Feature PR Submissions

While all feature PR submissions are welcome, please note that their merging is at ***sole discretion*** of the main developers.

When designing the new feature please take into account:
- **how they fit with the codebase/codestyle** (yes, we are aware the codebase has it's flaws - hence the rewrite efforts)
- **is the feature agnostic enough and will benefit all users**
- **the smaller the PR the better the chance of it being merged**

#### AI Usage

Fully "vibe-coded" submissions will most likely be rejected.

While personal opinions on AI usage and it's impact can vary person-to-person, we'd rather put the effort writing code than reviewing fully AI generated and copy-pasted code.

----

### Credits and Licensing

First and foremost, huge thanks for [Sam](https://github.com/LAB02-Admin) for creating and maintaining the original HASS.Agent in their spare time! We wouldn't be here withot the spark that pushed them to write the first line of code :heart:

As of now, we do not accept any kind of donation/coffee :)
<br>If you'd like however, you can support [creator](https://github.com/LAB02-Admin) of original HASS.Agent:

| [![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/lab02research) |  [!["Buy Me A Coffee"](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/lab02research) | [![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/donate/?hosted_button_id=5YL6UP94AQSPC) |
|:---:|---|---|

Thanks to the entire team that's developing [Home Assistant](https://www.home-assistant.io) - such an amazing platform!

The initial development was boosed by sleevezipper's [HASS Workstation Service](https://github.com/sleevezipper/hass-workstation-service). Thank you for sharing your hard work.

And a big thank you to all other packages:

[CoreAudio](https://github.com/morphx666/CoreAudio), [HotkeyListener](https://github.com/Willy-Kimura/HotkeyListener), [MQTTnet](https://github.com/chkr1011/MQTTnet), [Syncfusion](https://www.syncfusion.com), [Octokit](https://github.com/octokit/octokit.net), [Cassia](https://www.nuget.org/packages/Cassia.NetStandard/), [Grapevine](https://scottoffen.github.io/grapevine), [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor), [Newtonsoft.Json](https://www.newtonsoft.com/json), [Serilog](https://github.com/serilog/serilog), [CliWrap](https://github.com/Tyrrrz/CliWrap), [HADotNet](https://github.com/qJake/HADotNet), [Microsoft.Toolkit.Uwp.Notifications](https://github.com/CommunityToolkit/WindowsCommunityToolkit), [GrpcDotNetNamedPipes](https://github.com/cyanfish/grpc-dotnet-namedpipes), [gRPC](https://github.com/grpc/grpc), [ByteSize](https://github.com/omar/ByteSize).

Please consult their individual licensing if you plan to use any of their code.

Everything on the HASS.Agent platform is released under the [MIT license](https://opensource.org/licenses/MIT).

---

### Legacy

HASS.Agent is a .NET 8 application. If for some reason you can't install .NET 8, you can use the last .NET Framework 4.8 version also developed by Sam:

[v2022.3.8](https://github.com/LAB02-Research/HASS.Agent/releases/tag/v2022.3.8)

Per it's release time it was pretty feature complete if you just want commands, sensors, quickactions and notifications. 

You'll need to have .NET Framework 4.8 installed on your PC, which you can [download here](https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net48-web-installer).

#### Please note
- .NET Framework version is considered legacy and unsupported
- All issues/requests regarding the .NET Framework version of HASS.Agent will be closed