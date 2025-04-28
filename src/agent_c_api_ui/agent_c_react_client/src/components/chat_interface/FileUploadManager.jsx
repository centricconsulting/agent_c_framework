import React, { useRef } from 'react';
import { API_URL } from "@/config/config";
import { cn } from "@/lib/utils";
import { useMessageContext } from '@/contexts/MessageContext';

// Import our standardized FilesPanel
import FilesPanel from './FilesPanel';

/**
 * FileUploadManager handles all file upload functionality including selection,
 * uploading, tracking processing status, and integration with FilesPanel.
 * 
 * @component
 * @param {Object} props - Component props
 * @param {string} props.sessionId - Current session ID
 * @param {React.RefObject} [props.fileInputRef] - Optional ref for file input element
 * @param {Array} [props.uploadedFiles] - Array of uploaded files (optional, uses MessageContext if not provided)
 * @param {string} [props.className] - Additional CSS classes
 */
const FileUploadManager = ({
  sessionId,
  fileInputRef: externalFileInputRef,
  uploadedFiles: externalUploadedFiles,
  className
}) => {
  // Get message context for file state management
  const {
    selectedFiles,
    isUploading,
    setIsUploading,
    addSelectedFile,
    setSelectedFiles
  } = useMessageContext('FileUploadManager');
  
  // Use external ref if provided, or create our own
  const internalFileInputRef = useRef(null);
  const fileInputRef = externalFileInputRef || internalFileInputRef;

  /**
   * Toggles selection state of a file
   * @param {string} fileId - ID of the file to toggle
   */
  const toggleFileSelection = (fileId) => {
    // Toggle the selected state in our file list
    const updatedFiles = selectedFiles.map(file => {
      if (file.id === fileId) {
        return {...file, selected: !file.selected};
      }
      return file;
    });
    
    // Update selected files with new selection state
    setSelectedFiles(updatedFiles.filter(file => file.selected));
  };

  /**
   * Handles file selection from file input
   * @param {Event} e - Change event from file input
   */
  const handleFileSelection = (e) => {
    if (e.target.files && e.target.files.length > 0) {
      const file = e.target.files[0];
      console.log('FileUploadManager: File selected', file.name);
      
      // Automatically upload the file when selected
      setTimeout(() => {
        handleUploadFile(file);
      }, 10);
    }
  };

  /**
   * Opens the file picker dialog
   */
  const openFilePicker = () => {
    fileInputRef.current?.click();
  };

  /**
   * Handles file upload to the server and tracks processing status
   * @param {File} fileToUpload - File to upload
   * @returns {Promise<void>}
   * @throws {Error} If the file upload fails
   */
  const handleUploadFile = async (fileToUpload) => {
    if (!fileToUpload) {
      console.error('No file to upload!');
      return;
    }

    console.log('FileUploadManager: Starting upload of', fileToUpload.name);
    setIsUploading(true);

    const formData = new FormData();
    formData.append("ui_session_id", sessionId);
    formData.append("file", fileToUpload);

    try {
      console.log('Uploading file:', fileToUpload.name);
      // Upload the file
      const response = await fetch(`${API_URL}/upload_file`, {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        console.error(`Upload failed with status: ${response.status}`);
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      console.log('Upload successful:', data);

      // Add file to MessageContext
      const newFile = {
        id: data.id,
        name: data.filename,
        type: data.mime_type,
        size: data.size,
        selected: true,
        processing_status: "pending", // Initial status
        processing_error: null
      };

      // Add to selected files
      addSelectedFile(newFile);

      // Reset file input
      if (fileInputRef.current) {
        fileInputRef.current.value = null;
      }

    } catch (error) {
      console.error("Error uploading file:", error);
      // Reset file input
      if (fileInputRef.current) {
        fileInputRef.current.value = null;
      }
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <div className={cn("flex flex-col w-full", className)}>
      {/* Hidden file input */}
      <input
        type="file"
        ref={fileInputRef}
        onChange={handleFileSelection}
        className="hidden"
      />
      
      {/* Files panel showing uploaded files */}
      <FilesPanel 
        uploadedFiles={externalUploadedFiles || selectedFiles} 
        toggleFileSelection={toggleFileSelection} 
      />
      
      {/* Expose component API */}
      {React.Children.only(React.createElement('div', {
        className: "hidden",
        'data-file-manager-api': JSON.stringify({
          isUploading,
          fileCount: selectedFiles.length,
          selectedFileCount: selectedFiles.length,
        }),
      }))}
    </div>
  );
};

// Export methods for external use
FileUploadManager.openFilePicker = (ref) => {
  ref.current?.click();
};

export default FileUploadManager;