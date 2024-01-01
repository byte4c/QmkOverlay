QmkOverlay

I wanted a live view to see my layers on the fly and this does a good enough job.
There are 2 parts to the code; a driver that does HID usb communication and a console that does visualization.
While I'm happy with the driver code, the visualization code is to hardcoded.
In theory, you can retrieve the board layout for the device and use that.
