# TimeMonitor

TimeMonitor is a Unity asset designed to help developers track the amount of time spent working on a project. It provides detailed logs of work sessions and various visualization tools to understand the time investment in a project.

**_This asset is still very early in development_**

## Features

- **Detailed Session Tracking**: Logs every session with start and end times.
- **Monthly and Daily Logs**: Organizes sessions into monthly and daily logs.
- **Interactive Calendar View**: Visualizes the time spent on each day in a calendar format.
- **Line Graph and Histogram**: Provides graphical representations of time spent per day.
- **Session Management**: Combines multiple sessions of a day into one and clears all data easily via settings.

## How It Works

### Installation

1. Clone or download the `TimeMonitor` repository.
2. Place the `TimeMonitor` folder into your Unity project's `Assets` directory.

### Usage

1. **Opening the Time Monitor**:
    - Go to `Tools > Time Monitor` in the Unity menu to open the Time Monitor window.

2. **Tracking Time**:
    - The asset automatically tracks your sessions when the window is open. It logs the start and end times of each session. Closing the window or unity end a session that register automatically.

3. **Viewing Logs**:
    - The main window displays the current session time and total project time.
    - Use the buttons to switch between Calendar View, Line Graph, and Histogram.

4. **Settings**:
    - Click on the "Settings" button at the top of the window to open the Settings window.
    - Options in the Settings window:
        - **Combine All Sessions**: Combines multiple sessions of the same day into one to clean up data.
        - **Clear All Data**: Deletes all logged data.
     
Every session is logged in a friendly format in the sub data folder in a scriptable object. It allows you to easily add missing session or delete some. And pass data to a new unity project.
At the opening of the window the asset will look for that scriptable object, if none, one will be created.

### Visualizations

- **Calendar View**:
    - Displays a calendar for the selected month, showing the time spent on each day.
    
- **Line Graph**:
    - Plots the hours worked per day over the selected date range.
    
- **Histogram**:
    - Displays a bar chart of hours worked per day over the selected date range.

## Known Issues

- **_This asset is still very early in development_**
- **_Unsolved issue or actual limitation will be posted here_**

## Changelog

### Version 0.2

- Added a "Settings" button with options to combine sessions and clear all data.
- Implemented interactive calendar view.
- Added line graph and histogram visualizations.
- Enhanced session logging with monthly and daily organization.
- Improved UI for selecting date ranges and viewing logs.
- Make use of Scriptable objects to register data.
