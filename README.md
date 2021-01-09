# LaCrosse WS2300 weather station poller and twitter bot

This is a Twitter bot I made to tweet weather conditions from my home in Morningside, Whangārei,
Aotearoa New Zealand 🇳🇿. It can be found on Twitter at @morningsidewthr.

You should be able to run the code anywhere DotNet Core runs. I've tested it on macOS and Raspbian Linux.

## This project consists of

* A driver to communicate with a La Crosse WS2300 series weather station via RS232
* A Twitter bot to post periodic sensor readings

My weather station can be found on Twitter at @morningsidewthr, and is based near the top of a hill in Morningside, Whangārei,
Aotearoa New Zealand 🇳🇿

## Forseeably Asked Questions

* Why is the WS2300 driver so over-engineered? *

I pulled it from another data aquisition project of mine which needed to handle a much wider range of data sources, and so had a
more general interface. I pulled most of this complexity out of the WS2300 driver to simplify the interface for the Twitter bot,
but there's still some tidying up to be done

* What hardware are you using? *

I'm using a Raspberry Pi Model 3B and a Realtek-8192 clone USB-to-Serial converter. I'm waiting on serial level converter so I can
hook the weather station to the 3.3V serial port available on the rPi's GPIO pins.

* Where is the WS2300 protocol documented? *

I got all my info from the [Open2300](https://www.lavrsen.dk/foswiki/bin/view/Open2300/OpenWSAPI) page. It was reverse engineered,
is a pretty simplistic (and unreliable) protocol, essentially just providing raw access to the weather station's RAM.

The Open2300 project is no longer maintained and is looking for someone to take over. I don't like writing C for stuff like this
any more, but some day I'd like to document the protocol here in case the WS2300 site vanishes.

## Thanks

Kenneth Lavrsen for the [Open2300 project](https://www.lavrsen.dk/foswiki/bin/view/Open2300/OpenWSAPI)
