// callgettoken.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

 #include <windows.h>
#include <iostream>
#include <fstream>
#include <string>
#include <iomanip>
#include <thread>

int main()
{
    while (true)
    {                
        std::wcout << "Called get token " << std::endl;
        std::wstring filename = L"\\\\.\\pipe\\swaptoken";
        std::wstring str;

        std::wfstream pipe_stream;

        pipe_stream.open(filename, std::ifstream::in | std::ifstream::out);

        pipe_stream << "Hello!" << std::endl;
        pipe_stream.flush();

        std::getline(pipe_stream, str);

        std::wcout << L"Token " << str << std::endl;

        pipe_stream.close();
        
        std::wcout << L"Sleeping for 60s... ";
        
        ::Sleep(600000);
    }
}

