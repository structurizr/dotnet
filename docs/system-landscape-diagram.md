# System Landscape diagram

A System Landscape diagram is really the same as the [System Context diagram](system-context-diagram.md), without a focus on a specific software system. It can help to provide a broader view of the people and software systems that are related to and reside within a given enterprise (e.g. a business or organisation).

## Example

As an example, a System Landscape diagram for a simplified, fictional Internet Banking System might look something like this. In summary, it shows more than just the immediate relationships of the Internet Banking System.

![An example System Landscape diagram](images/system-landscape-diagram-1.png)

With Structurizr for .NET, you can create this diagram with code like the following:

```c#
Person customer = model.AddPerson(Location.External, "Customer", "A customer of the bank.");
    
SoftwareSystem internetBankingSystem = model.AddSoftwareSystem(Location.Internal, "Internet Banking System", "Allows customers to view information about their bank accounts and make payments.");
customer.Uses(internetBankingSystem, "Uses");
    
SoftwareSystem mainframeBankingSystem = model.AddSoftwareSystem(Location.Internal, "Mainframe Banking System", "Stores all of the core banking information about customers, accounts, transactions, etc.");
internetBankingSystem.Uses(mainframeBankingSystem, "Uses");
    
SoftwareSystem atm = model.AddSoftwareSystem(Location.Internal, "ATM", "Allows customers to withdraw cash.");
atm.Uses(mainframeBankingSystem, "Uses");
customer.Uses(atm, "Withdraws cash using");
    
Person bankStaff = model.AddPerson(Location.Internal, "Bank Staff", "Staff within the bank.");
bankStaff.Uses(mainframeBankingSystem, "Uses");

EnterpriseContextView enterpriseContextView = views.CreateSystemLandscapeView("EnterpriseContext", "The system landscape diagram for the Internet Banking System.");
enterpriseContextView.AddAllElements();
```

See [BigBankPlc.cs](https://github.com/structurizr/dotnet/blob/master/Structurizr.Examples/BigBankPlc.cs) for the full code, and [https://structurizr.com/share/36141#EnterpriseContext](https://structurizr.com/share/36141#EnterpriseContext) for the diagram.