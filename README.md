# AuthProxy

Unlike Controller Actions, AuthProxy is a library developed to provide authorization on objects.
With AuthProxy, you can add user authorization to the object itself or its methods.

## Get Started

**Startup.cs**
```csharp
public void ConfigureServices(IServiceCollection services)  
{
	...
	services.AddObjectBasedAuthorization();
	...
}
```

**Example SignIn and Permission Registerar**

```csharp
public async Task SignInAsync(string email,string name){
	var claims = new List<Claim>  
	{  
	  new Claim(ClaimTypes.Name, name),  
	  new Claim(ClaimTypes.Email, email),  
	};
	 
	var claimsIdentity = new ClaimsIdentity(  
	 claims, CookieAuthenticationDefaults.AuthenticationScheme);  

	var usersUniqueNamedPermissions = new List<string>(){
		"Withdraw_Money",
		"Deposit_Money",
	};

	claimsIdentity.RegisterPermissions(usersUniqueNamedPermissions);

	var authProperties = new AuthenticationProperties  
	{  
	  AllowRefresh = true  
	};  
	  
	await HttpContext.SignInAsync(  
	  CookieAuthenticationDefaults.AuthenticationScheme,  
	  new ClaimsPrincipal(claimsIdentity),  
	  authProperties);
}

```

**Example Authorized Object | Service**
```csharp
public interface IAuthorizedService : IAuthorizedObject  
{  
  void WithdrawMoney(double amount);  
  void DepositMoney(double amount);  
}

[Authorize("HasBankCard")]  
public class AuthorizedService : IAuthorizedService  
{  
	 [Authorize("Withdraw_Money")]  
	 public void WithdrawMoney(double amount)  
	 {  
		Console.WriteLine($"Withdraw money, ${amount}");  
	 }  
	 
	 [Authorize("Deposit_Money")]  
	 public void DepositMoney(double amount)  
	 {  
		Console.WriteLine($"Deposit money, ${amount}");  
	 }
}

```

When we look at the example above, the user will not be able to access the methods in the AuthorizedService object because we do not define HasBankCard authorization when the user logs in.

**NOTE: AuthProxy will first consider class-based authorizations and then method-based authorizations.**
