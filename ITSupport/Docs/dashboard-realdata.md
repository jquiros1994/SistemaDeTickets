# Dashboard — Connect Stats to Real Database

## What we changed and why

### Goal
Replace the four hardcoded numbers (Open Cases, Critical Priority, Resolved Today, Active Engineers)
and the fake Recent Cases list in the Dashboard with live data from `TicketSystemDB`.

---

## Files touched

| File | Change |
|------|--------|
| `Models/CaseViewModel.cs` | Added `CustomerName` property |
| `DAL/DashboardRepository.cs` | **New file** — all dashboard queries |
| `Controllers/DashboardController.cs` | GET Index now calls the repository |
| `Views/Dashboard/Index.cshtml` | `c.Customer` → `c.CustomerName` |
| `ITSupport.csproj` | Registered `DAL\DashboardRepository.cs` |

---

## Step 1 — Add `CustomerName` to `CaseViewModel`

`Models/CaseViewModel.cs` already had `OwnerName`, `ProgramName`, etc.
We added one line:

```csharp
public string CustomerName { get; set; }
```

The `Cases` table does not store the customer name directly — it goes through:
`Cases → Contacts (ContactId) → Customers (CustomerId) → FirstName + LastName`

---

## Step 2 — Create `DAL/DashboardRepository.cs`

Four scalar queries + one list query:

### `GetOpenCasesCount()`
```sql
SELECT COUNT(*)
FROM   Cases c
JOIN   CaseStatuses cs ON cs.StatusId = c.StatusId
WHERE  cs.IsOpen = 1
```

### `GetCriticalCasesCount()`
```sql
SELECT COUNT(*)
FROM   Cases c
JOIN   CaseStatuses cs ON cs.StatusId  = c.StatusId
JOIN   Priorities   p  ON p.PriorityId = c.PriorityId
WHERE  cs.IsOpen = 1
AND    p.PriorityName = 'Critical'
```

### `GetResolvedTodayCount()`
```sql
SELECT COUNT(*)
FROM   Cases c
JOIN   CaseStatuses cs ON cs.StatusId = c.StatusId
WHERE  cs.StatusName IN ('Resolved', 'Closed')
AND    CAST(c.UpdatedAt AS DATE) = CAST(GETUTCDATE() AS DATE)
```

### `GetActiveEngineersCount()`
```sql
SELECT COUNT(*) FROM SupportEngineers WHERE IsActive = 1
```

### `GetRecentCases(int top = 10)`
Returns `List<CaseViewModel>` with a full JOIN that includes CustomerName:

```sql
SELECT TOP (@Top)
       c.CaseId, c.CaseNumber, c.Title,
       cs.StatusName,
       p.PriorityName,
       ct.ContactName,
       cust.FirstName + ' ' + cust.LastName AS CustomerName,
       se.Name  AS OwnerName,
       pr.ProgramName,
       c.Country, c.CreatedAt, c.UpdatedAt, c.ClosedAt
FROM   Cases c
JOIN   CaseStatuses    cs   ON cs.StatusId   = c.StatusId
JOIN   Priorities      p    ON p.PriorityId  = c.PriorityId
JOIN   Contacts        ct   ON ct.ContactId  = c.ContactId
JOIN   Customers       cust ON cust.CustomerId = ct.CustomerId
JOIN   Programs        pr   ON pr.ProgramId  = c.ProgramId
LEFT JOIN SupportEngineers se ON se.EngineerId = c.OwnerId
ORDER BY c.CreatedAt DESC
```

---

## Step 3 — Update `DashboardController` GET Index

Replaced the four `ViewBag.*` hardcoded integers and fake `RecentCases` list
with calls to the new repository:

```csharp
private readonly DashboardRepository _dashboard = new DashboardRepository();

// GET /Dashboard
[HttpGet]
public ActionResult Index()
{
    if (Session["UserId"] == null)
        return RedirectToAction("Index", "Home");

    ViewBag.Username        = Session["Username"];
    ViewBag.Role            = Session["Role"];
    ViewBag.OpenCases       = _dashboard.GetOpenCasesCount();
    ViewBag.CriticalCases   = _dashboard.GetCriticalCasesCount();
    ViewBag.ResolvedToday   = _dashboard.GetResolvedTodayCount();
    ViewBag.ActiveEngineers = _dashboard.GetActiveEngineersCount();
    ViewBag.RecentCases     = _dashboard.GetRecentCases(10);

    return View();
}
```

---

## Step 4 — Fix the View

In `Views/Dashboard/Index.cshtml` the Recent Cases loop used `c.Customer`
(the old ExpandoObject field name). Changed to `c.CustomerName` to match
the `CaseViewModel` property added in Step 1.

---

## Notes

- All DB access follows the existing ADO.NET pattern via `DatabaseHelper.GetConnection()`.
- `DashboardRepository` must be registered in `ITSupport.csproj` as a `<Compile>` entry,
  same as the other DAL files.
- If there are no cases yet in the DB the stats will correctly show `0`.
