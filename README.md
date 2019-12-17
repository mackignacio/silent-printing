# C# Print Receipt Console Application

This is a simple application for printing a receipt using C#. You use this as an external app for silent printing without showing any print dialog box.

## How to use

- Build the app
- Run the app via terminal with this command "print.exe args[0] args[1] args[2] args[3] args[4] args[5] args[6]"

## Arguments
> **NOTE** : Separate each arguments with space.

- **args[0]** : Business Logo (url)
- **args[1]** : Business Name
- **args[2]** : Business Address
- **args[3]** : Business Email
- **args[4]** : Invoice Details (Separated by ",")
  - **4.1** : Receipt Number
  - **4.2** : Date
  - **4.3** : Cashier Name
  - **4.4** : POS I.D. (Optional)
  - **4.5** : Customer Name (Optional)
- **args[5]** : Payment Details (Separated by ",")
  - **5.1** : Total Price
  - **5.2** : Tax
  - **5.3** : Discounts
  - **5.4** : Grand Total
  - **5.5** : Amount
  - **5.6** : Change
- **args[6]** : List of Items (Must be a JSON String)
  - **6.1** : Name
  - **6.2** : Quantity
  - **6.3** : Price