# You Copied That! V1.0.1

A lightweight utility that monitors the clipboard for Ctrl+C (copy) events and displays a popup notification when text is copied. Built with C# and Windows Forms

**Why I made this?**

I made this because sometimes I hit Ctrl+C and I swear I think it worked, but it didn't for some reason. Now I know for sure when it works!

**One thing though ...**

Trust the app. Seriously. Just trust it.

(Disobedience will be noted 👀)

---

## Current State of the App

The app is pretty simple right now, but it does what it's supposed to. Here's what's in it:

1. **CtrlCListener.cs**:

   - Listens for clipboard updates and checks if any copy command is given.
   - Displays a popup notification when new text is copied.

2. **PopupForm.cs**:

   - Creates a cool little popup with rounded corners and smooth animations.
   - Slides in from the bottom of the screen, fades in, and fades out after a second.

3. **SystemAccentColor.cs**:

   - Grabs the system's accent color to make the popup look nice and match your theme.

4. **Program.cs**:
   - The entry point of the app. Just starts everything up.

---

## How to Make It Start Up via Registry

If you want the app to start automatically when you turn on your computer, you can add it to the **Windows Startup** programs using the registry. Here's how:

### Steps:

1. Download the `bin\Debug\net8.0-windows` build files first:

   - Transfer the files in a folder to your preferred directory.

2. Open the Registry Editor:

   - Press `Win + R`, type `regedit`, and hit Enter.

3. Go to this key:

   ```
   HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run
   ```

4. Add a new string value:
   - Right-click on the right pane, select `New > String Value`.
   - Name it something like `YouCopiedThat`.
   - Set the value to the full path of the app's executable (e.g., `C:\Path\To\CtrlCPopup.exe`).

---

## Future Plans to Expand

I’ve got some ideas for making this app even better. Here’s what I’m thinking:

### **Core Improvements**:

1. **Error Handling**:

   - Add some error handling so the app doesn’t crash if something goes wrong with the clipboard.

2. **Multi-Monitor Support**:

   - Make the popup appear on the monitor where you’re actually working.

3. **Customization Options**:

   - Add a settings screen so you can tweak the popup’s appearance, position, and how long it stays visible.

4. **Custom Shortcuts**:

   - Let you change the shortcut from Ctrl+C to something else, like Ctrl+Shift+C.

5. **Sound Notifications**:

   - Add an option to play a sound when you copy something.

6. **Ignore Specific Apps**:

   - Let you exclude certain apps from triggering the popup (like if you don’t want it popping up while you’re gaming or something).

7. **System Tray Integration**:
   - Add an icon to the system tray so you can quickly open settings or turn the app on/off.

### **UI/UX Improvements**:

1. **Modern Settings Screen**:

   - Make a settings screen that looks clean and modern, kinda like [PowerToys](https://github.com/microsoft/PowerToys).

2. **More Animations**:

   - Add more animation options for the popup, like sliding in from different directions or fading in differently.

3. **Dark/Light Mode**:
   - Add support for dark and light themes based on your system settings.

---

## How to Contribute

If you want to help out with this project, feel free to fork the repo and submit a pull request. Here are some areas where you could contribute:

- Adding new features.
- Improving the UI/UX.
- Adding error handling and making the app more robust.
- Writing tests.

---

Let me know if you have any other ideas or if you want to tweak anything!
