# Country Search Plugin for PowerToys Run

A [PowerToys Run](https://aka.ms/PowerToysOverview_PowerToysRun) plugin for looking up information about countries.

## Features

### Search a country

You can search up a country by giving info you see about it. It includes : 
* Domain
* Phone code
* Driving side
* Flag colors
* Flag features

## Installation

### Manual

1. Download the latest release of the from the releases page.
2. Extract the zip file's contents to `%LocalAppData%\Microsoft\PowerToys\PowerToys Run\Plugins`
3. Restart PowerToys.

### Via [ptr](https://github.com/8LWXpg/ptr)

```shell
ptr add CountrySearch RakambdaOrg/PowerToys-Run-CountrySearch
```

## Usage

1. Open PowerToys Run (default shortcut is <kbd>Alt+Space</kbd>).
2. Type `cs` and add as many search terms you want.

## Building

1. Clone the repository.
2. run `dotnet build -c Release`.
