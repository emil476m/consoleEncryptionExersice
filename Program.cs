// See https://aka.ms/new-console-template for more information

using System.Collections;
using System.Security.Cryptography;
using System.Text;

Console.ForegroundColor = ConsoleColor.Green;
Console.BackgroundColor = ConsoleColor.Black;


var key = new byte[32];

setup();


void setup()
{
    Console.WriteLine("_____________________________________________________" + "\n" +
                      "1: Enter Password" + "\n" + "2: Create new password" + "\n" + 
                      "_____________________________________________________");
    string? enteredText = Console.ReadLine();
    if (enteredText.Equals("1"))
    {
        enterPassword();
    }
    else if (enteredText.Equals("2"))
    {
        createPassword();
    }
    
}

void enterPassword()
{
    Console.WriteLine(
                      "Please Enter Password" + "\n" + 
                      "_____________________________________________________");
    string? enteredText = Console.ReadLine();
    try
    {
        byte[] bytesList = Encoding.Default.GetBytes(enteredText);
        
        key = bytesList;
        
        
       /* // generate a slat
        var salt = new byte[32];
        RandomNumberGenerator.Fill(salt);
    
        //make key
        var key = KeyDerivation.Pbkdf2(
            password: getPassword()!,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 600000,
            numBytesRequested: 256 / 8);*/
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }

    if (key.Length> 0)
    {
        menu();
    }
    else
    {
        Console.WriteLine("_____________________________________________________"+ "\n" +"Not a valid password!"+ "\n" + 
                          "_____________________________________________________");
        enterPassword();
    }
}

void createPassword()
{
    Console.WriteLine("not implemented");
}

void getSalted()
{
    
}

void menu()
{
    Console.WriteLine("_____________________________________________________" + "\n" +
        "1: Safely store message" + "\n" + "2: Read message" + "\n" + "3: Exit" + "\n" + 
        "_____________________________________________________");
    menuListener();
}

void menuListener()
{
    string? enteredText = Console.ReadLine();
    if (enteredText.Equals("1"))
    {
        writeInFile();
    }
    else if (enteredText.Equals("2"))
    {
        readFromFile();
    }
    else if (enteredText.Equals("3"))
    {
        Console.WriteLine("Stopping Program");
        Environment.Exit(1);
    }
    else
    {
        Console.WriteLine("\n" + "Please Enter a valid option" +"\n");
        menu();
    }
}

void writeInFile()
{
    Console.WriteLine("Please Write the desired message:");
    
    string? enteredText = Console.ReadLine();
    
    
    string encryptedMessage = encrypt(enteredText);
    
    // Set a variable to the Documents path.
    string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    // Append text to an existing file named "WriteLines.txt".
    using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "C:\\Users\\emilw\\IdeaProjects\\consoleEncryptionExersice\\message.txt"), true))
    {
        outputFile.WriteLine(encryptedMessage);
    }
    menu();
}

void readFromFile()
{
    try
    {
        string filePath = "C:\\Users\\emilw\\IdeaProjects\\consoleEncryptionExersice\\message.txt";
        string output = "";
        // Open the text file using a stream reader.
        using (var sr = new StreamReader(filePath))
        {
            // Read the stream as a string, and write the string to the console.
            output = sr.ReadToEnd();
            
        }
        
        output = output.Replace("\r\n", "");
        
        string[] list = output.Split(",");
        
        for (int i = 0; i+1 <= (list.Length/3); i++)
        {
            if (i==0)
            {
                Console.WriteLine(decrypt(Convert.FromHexString(list[i]), Convert.FromHexString(list[i+1]), Convert.FromHexString(list[i+2]), key));
            }
            else
            {
                Console.WriteLine(decrypt(Convert.FromHexString(list[i*3]), Convert.FromHexString(list[i*3+1]), Convert.FromHexString(list[i*3+2]), key));
            }
            
        }
        
    }
    catch (IOException e)
    {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(e.Message);
    }
    
    menu();
}



string encrypt(string input)
{
    using var aes = new AesGcm(key);

    var nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // MaxSize = 12
    RandomNumberGenerator.Fill(nonce);

    var plaintextBytes = Encoding.UTF8.GetBytes(input);
    var ciphertext = new byte[plaintextBytes.Length];
    var tag = new byte[AesGcm.TagByteSizes.MaxSize]; // MaxSize = 16
    
    aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);
    
    return (Convert.ToHexString(nonce) + "," + Convert.ToHexString(tag) + "," + Convert.ToHexString(ciphertext))+",";
}

string decrypt(byte[] nonce, byte[] tag, byte[] ciphertext, byte[] key)
{
    
    using (var aes = new AesGcm(key))
    {
        var plaintextBytes = new byte[ciphertext.Length];

        aes.Decrypt(nonce, ciphertext, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }
}