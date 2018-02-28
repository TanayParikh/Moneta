# Moneta FMS 2.0
## Scope
### Initial Scope
- Rewrite core application using UWP to improve overall user experience
	- Updates to core UI/UX
	- Replace the portable LAMP server needed for MYSQL with SQLite
	- Increase overall security. 
		- Authentication on start
		- Encrypted DB
	- Add non-intrusive, privacy friendly telemetrics to further improve UX	
	- Autoupdater
	- High test coverage

#### General Notes
- UWP will permit direct distribution through the Microsoft Store

### Future Plans
1. Private, self-hosted server capabilities
2. **Native** cross platform support
	a. Android
	b. iOS / MacOS

## Development Notes
**Why `Decimal` over `Double`?**
https://stackoverflow.com/questions/3730019/why-not-use-double-or-float-to-represent-currency/3730040#3730040
