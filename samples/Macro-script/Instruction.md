# Script Like a Macro

*Script like a macro* is a feature of the **Grasshopper Application** that lets you quickly run your Grasshopper scripts without manually reloading settings in the Grasshopper Application window.

## How It Works

This feature relies on an already-opened instance of the Grasshopper Application. Open it once, minimize it or move it to an unused corner of your screen, then trigger the macro. The macro will instruct the open instance to do its job.

## Configuration

A sample macro is provided in the `Grasshopper - Macro sample.cs` file. You'll need to adapt its content to your needs (it's a plain text file, so Notepad is sufficient). Look for this line:

​```
NewProcess.StartInfo.Arguments = "-f \"SETTINGS.DrawingLink.UI.MainWindow.xml\"";
​```

This is the anchor point between your macro and the Grasshopper Application. Change it to reference the pre-saved settings of the Grasshopper Application (visible as a combobox at the top of the UI), which control both the script path and the input values.