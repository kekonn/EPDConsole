# EPD Console exercise and solution

## Considerations

For brevity and overview, certain layers of architecture are now simple folders in the main program, that would have been separate class library projects in a larger, full scale project (i.e. Models). 
I have also opted to use the DBContext directly instead of a Unit of Work pattern for simplicity's sake.

Certain input validations are also skipped, that are highly recommended in production code, such as verifying the INSZ number and trimming whitespace at the start and end of certain string inputs (or code page conversion).

I have also not provided a way to add availabilities to doctors, which would make the appointment scheduling far more complex. But in a realistic scenario this would definitely be needed.