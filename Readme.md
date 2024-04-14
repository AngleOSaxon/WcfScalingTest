# WCF Scaling Tests

This is a very simple repo attempting to reproduce issues making multiple simultaneous WCF requests from a .NET Core (or .NET 5+) client.  The issue appears to be that a certain number of simultaneous requests will perform *very* poorly, taking much longer than the equivalent code in .NET Framework.

# Usage

I built this mostly as some personal throwaway code, so there's some hardcoded stuff you'll need to change to run it locally.  I don't expect it to have a life expectancy long enough to be worth fixing, to be honest.  


## Pre-running work:

1. The `WcfScalingTestServer` and `WcfScalingTestServer_Framework` projects both reference a cert via thumbprint in order to use TLS on the connection.  You'll need to change that thumbprint to one available in your cert store, or load a cert another way.  The cert can be self-signed and invalid, but it neds to exist and you need to have the private key.
2. Replace `aiConnectionString` with an empty string in `WcfScalingTestClient`'s `Program.cs`.  Or use your own AppInsights connection string if you want.  If you don't do this, nothing will break, but your computer name may end up in an App Insights log unless I actually remember to clean up that resource.
3. Adjust the port numbers on the server endpoints if you want.  They're currently all 3001.

## Running the tests

1. Build and run the `WcfScalingTestServer_Framework` project.  I recommend doing this separately in a console window instead of in your IDE--the server won't have any useful information, it's just there to give the clients something to talk to.
2. Build and run a Benchmark app, either Framework or not.  This will communicate with the server from step 1 using Streaming or Buffered transport modes, and using async and sync calls.

# Structure

There is a Contract library that just defines the contract shared between all the implementations.  The Server apps run WCF servers that offer a single operation and wait 50ms before returning, to simulate doing some actual work.  The Client apps use the Contract libraries to spin up 500 clients and make three calls each before closing.  The Metrics library is leftover from when I didn't understand the issue and wanted to play with event counters and can generally be ignored.