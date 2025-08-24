# UntBookmark
A solution for using Bookmark feature with Fake IP.

## Usage

1. Deploy one of web servers in `servers` (choose as you prefer)
2. Install the Unturned server plugin
3. Configure your Unturned server to use Fake IP, set `BookmarkHost` to the deployed web server URL, and set `Login_Token` properly
4. Done

### BookmarkHost Format

- https://example.com/?server=server1
- https://example.com/s/server1

## Installation

### OpenMod

Copy `UntBookmark.Shared.dll` and `UntBookmark.OpenMod.dll` to `OpenMod/plugins` folder.

### Rocket

Copy `UntBookmark.Rocket.dll` to `Rocket/Plugins` folder.

## Deploy

### Cloudflare Worker

1. Install `wrangler`
2. Configure your KV binding
3. Add your Unturned server login tokens to `GSLTS` secret (environment variable), if there are multiple ones, separate with `,`
3. Run `wrangler deploy`
