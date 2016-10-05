using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiDemo1.Models;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Web.Mvc;
using System.Runtime.InteropServices;

namespace WebApiDemo1.Controllers
{
    public class NotesController : ApiController
    {
        private List<Note> Notes;
        private string _dataPath;
        private string _logFilePath;
        private string _encryptedFileName;
        private string _nonEncryptedFileName;

        public NotesController()
        {
            _nonEncryptedFileName = "saved_notes.json";
            _encryptedFileName = "enc_" + _nonEncryptedFileName;

            Notes = new List<Note>();
            _dataPath = "/notes_app_data/";
            _logFilePath = _dataPath + "log.txt";

            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }else 
            {
                
                LoadNotesFromFile();
            }
        }

        public IHttpActionResult CreateNote([FromBody] Note newNote)
        {
            if (LoadNotesFromFile())
            {
                
                if (Notes.Count == 0)
                {
                    newNote.Id = 1;

                }
                else
                {
                    newNote.Id = Notes.Count + 1;
                }

                Notes.Add(newNote);
            }
            else
            {
                newNote.Id = 1;

                Notes.Add(newNote);

            }

            string json = JsonConvert.SerializeObject(Notes);
            
            File.WriteAllText(_dataPath + _nonEncryptedFileName, json);

            return Ok(newNote.Id);
        }

        public IEnumerable<Note> GetAllNotes()
        {
            return Notes;
        }

        public IHttpActionResult GetNote([FromUri] int id, string password)
        {
            Note note = null;

            if (LoadNotesFromFile())
            {
                note = Notes.FirstOrDefault( n => n.Id == id && n.Password == password);
                if (note == null)
                {
                    return NotFound();
                }
            }else
            {
                return NotFound();
            }
      
            return Ok(note);  
        }

        
        private bool LoadNotesFromFile()
        {
            

            if (File.Exists(_dataPath + _nonEncryptedFileName))
            {
                using (StreamReader reader = new StreamReader(_dataPath + _nonEncryptedFileName))
                {
                    try
                    {
                        string jsonData = reader.ReadToEnd();
                        Notes = JsonConvert.DeserializeObject<List<Note>>(jsonData);
                        reader.Close();
                    }
                    catch (Exception ex)
                    {

                        File.AppendAllText(_logFilePath, string.Format("Error loading file: {0}{1}",  Environment.NewLine, ex.Message));
                    }
                    
                }

                return true;
            }

            return false;
        }


        
        private bool EncryptFile()
        {
            string nonEncryptedFilePath = _dataPath + _nonEncryptedFileName;
            string encryptedFilePath = _dataPath + _encryptedFileName;

            //Distribute key
            string sSecretKey;
            //Get secret key
            sSecretKey = FileEncryptDecrypt.GenerateKey();
            //For additional security pin the key
            GCHandle gch = GCHandle.Alloc(sSecretKey, GCHandleType.Pinned);

            //encrypt file
            FileEncryptDecrypt.EncryptFile(nonEncryptedFilePath, encryptedFilePath, sSecretKey);
                        
            //delete non-ecrypted file after successful encryption
            File.Delete(nonEncryptedFilePath);
            //write to log file
            var data = DateTime.Now.ToString()  +  " " + _nonEncryptedFileName + Environment.NewLine;
            File.AppendAllText(_logFilePath, "File encrypted: " + data);

            if (File.Exists(_dataPath + _encryptedFileName))
            {
                return true;
            }

            return false;
            
        }

        private bool DecryptFile()
        {       
            //Distribute key
            string sSecretKey;
            //Get secret key
            sSecretKey = FileEncryptDecrypt.GenerateKey();
            //For additional security pin the key
            GCHandle gch = GCHandle.Alloc(sSecretKey, GCHandleType.Pinned);

            //decrypt
            FileEncryptDecrypt.DecryptFile(_dataPath + _encryptedFileName, _dataPath + _nonEncryptedFileName, sSecretKey);
            //delete non-ecrypted file after successful encryption
            File.Delete(_dataPath + _encryptedFileName);
            //write to log file
            var data = DateTime.Now.ToString() + " " + _nonEncryptedFileName + Environment.NewLine;
            File.AppendAllText(_logFilePath, "File decrypted: " + data);

            //if file exists
            if (File.Exists(_dataPath + _nonEncryptedFileName))
            {
                return true;
            }

            return false;
        }
    }
}
