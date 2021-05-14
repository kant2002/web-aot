Web AOT
=======

This repository was inspired by amazing work of @LLT21 which he presents at https://github.com/dotnet/aspnetcore/issues/31561 and his repo https://github.com/LLT21/NativeAOT5 where most of this piecese presented.

What I have is simplest app which work out of the box.

So far issues with this approach
- Require local patching of ASP.NET Core for HTTPS support (one time step)
- No dependency injection.
- No routing
- Not tested configuration
- Logging behave not NativeAOT friendly.

Plans
- Connection to databases (non-ODBC)
- source generators for routing