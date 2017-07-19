using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ESH.Utility
{
    /// <summary>
    /// Class to deal with NTFS inheritance
    /// </summary>
    public static class NtfsInheritance
    {
        /// <summary>
        /// Calculate access rule inheritance
        /// </summary>
        public class AccessRuleInheritance
        {
            private FileSystemAccessRule _AccessRule;
            public FileSystemAccessRule AccessRule { get { return this._AccessRule; } }
            private InheritanceType _InheritanceType;
            public InheritanceType InheritanceType { get { return this._InheritanceType; } }
            private bool? _ApplyToThisContainerOnly;
            public bool? ApplyToThisContainerOnly { get { return this._ApplyToThisContainerOnly; } }

            /// <summary>
            /// Calculate access rule inheritance for a given file system access rule
            /// </summary>
            /// <param name="accessRule">Access rule to analyze</param>
            public AccessRuleInheritance(FileSystemAccessRule accessRule)
            {
                this._AccessRule = accessRule;
                if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_ThisFolderOnlyX && this._AccessRule.PropagationFlags == NtfsInheritance.PF_ThisFolderOnlyX)
                {
                    this._InheritanceType = InheritanceType.ThisFolderOnly;
                    this._ApplyToThisContainerOnly = true;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_ThisFolderSubFoldersAndFiles && this._AccessRule.PropagationFlags == NtfsInheritance.PF_ThisFolderSubFoldersAndFiles)
                {
                    this._InheritanceType = InheritanceType.ThisFolderSubFoldersAndFiles;
                    this._ApplyToThisContainerOnly = false;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_ThisFolderSubFoldersAndFilesX && this._AccessRule.PropagationFlags == NtfsInheritance.PF_ThisFolderSubFoldersAndFilesX)
                {
                    this._InheritanceType = InheritanceType.ThisFolderSubFoldersAndFiles;
                    this._ApplyToThisContainerOnly = true;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_ThisFolderAndSubfolders && this._AccessRule.PropagationFlags == NtfsInheritance.PF_ThisFolderAndSubfolders)
                {
                    this._InheritanceType = InheritanceType.ThisFolderAndSubfolders;
                    this._ApplyToThisContainerOnly = false;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_ThisFolderAndSubfoldersX && this._AccessRule.PropagationFlags == NtfsInheritance.PF_ThisFolderAndSubfoldersX)
                {
                    this._InheritanceType = InheritanceType.ThisFolderAndSubfolders;
                    this._ApplyToThisContainerOnly = true;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_ThisFolderAndFiles && this._AccessRule.PropagationFlags == NtfsInheritance.PF_ThisFolderAndFiles)
                {
                    this._InheritanceType = InheritanceType.ThisFolderAndFiles;
                    this._ApplyToThisContainerOnly = false;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_ThisFolderAndFilesX && this._AccessRule.PropagationFlags == NtfsInheritance.PF_ThisFolderAndFilesX)
                {
                    this._InheritanceType = InheritanceType.ThisFolderAndFiles;
                    this._ApplyToThisContainerOnly = true;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_SubfoldersAndFilesOnly && this._AccessRule.PropagationFlags == NtfsInheritance.PF_SubfoldersAndFilesOnly)
                {
                    this._InheritanceType = InheritanceType.SubfoldersAndFilesOnly;
                    this._ApplyToThisContainerOnly = false;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_SubfoldersAndFilesOnlyX && this._AccessRule.PropagationFlags == NtfsInheritance.PF_SubfoldersAndFilesOnlyX)
                {
                    this._InheritanceType = InheritanceType.SubfoldersAndFilesOnly;
                    this._ApplyToThisContainerOnly = true;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_SubfoldersOnly && this._AccessRule.PropagationFlags == NtfsInheritance.PF_SubfoldersOnly)
                {
                    this._InheritanceType = InheritanceType.SubfoldersOnly;
                    this._ApplyToThisContainerOnly = false;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_SubfoldersOnlyX && this._AccessRule.PropagationFlags == NtfsInheritance.PF_SubfoldersOnlyX)
                {
                    this._InheritanceType = InheritanceType.SubfoldersOnly;
                    this._ApplyToThisContainerOnly = true;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_FilesOnly && this._AccessRule.PropagationFlags == NtfsInheritance.PF_FilesOnly)
                {
                    this._InheritanceType = InheritanceType.FilesOnly;
                    this._ApplyToThisContainerOnly = false;
                }
                else if (this.AccessRule.InheritanceFlags == NtfsInheritance.IF_FilesOnlyX && this._AccessRule.PropagationFlags == NtfsInheritance.PF_FilesOnlyX)
                {
                    this._InheritanceType = InheritanceType.FilesOnly;
                    this._ApplyToThisContainerOnly = true;
                }
                else
                {
                    this._InheritanceType = InheritanceType.Other;
                    this._ApplyToThisContainerOnly = null;
                }

            }
        }

        /// <summary>
        /// Inheritance types
        /// </summary>
        public enum InheritanceType
        {
            ThisFolderOnly,
            ThisFolderSubFoldersAndFiles,
            ThisFolderAndSubfolders,
            ThisFolderAndFiles,
            SubfoldersAndFilesOnly,
            SubfoldersOnly,
            FilesOnly,
            Other
        }

        //X means apply to this container only
        //Note: ThisFolderOnly by definition always has the "apply to this container only" checked
        public static readonly InheritanceFlags IF_ThisFolderOnlyX = InheritanceFlags.None;
        public static readonly PropagationFlags PF_ThisFolderOnlyX = PropagationFlags.None;

        public static readonly InheritanceFlags IF_ThisFolderSubFoldersAndFiles = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
        public static readonly PropagationFlags PF_ThisFolderSubFoldersAndFiles = PropagationFlags.None;

        public static readonly InheritanceFlags IF_ThisFolderSubFoldersAndFilesX = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
        public static readonly PropagationFlags PF_ThisFolderSubFoldersAndFilesX = PropagationFlags.NoPropagateInherit;

        public static readonly InheritanceFlags IF_ThisFolderAndSubfolders = InheritanceFlags.ContainerInherit;
        public static readonly PropagationFlags PF_ThisFolderAndSubfolders = PropagationFlags.None;

        public static readonly InheritanceFlags IF_ThisFolderAndSubfoldersX = InheritanceFlags.ContainerInherit;
        public static readonly PropagationFlags PF_ThisFolderAndSubfoldersX = PropagationFlags.NoPropagateInherit;

        public static readonly InheritanceFlags IF_ThisFolderAndFiles = InheritanceFlags.ObjectInherit;
        public static readonly PropagationFlags PF_ThisFolderAndFiles = PropagationFlags.None;

        public static readonly InheritanceFlags IF_ThisFolderAndFilesX = InheritanceFlags.ObjectInherit;
        public static readonly PropagationFlags PF_ThisFolderAndFilesX = PropagationFlags.NoPropagateInherit;

        public static readonly InheritanceFlags IF_SubfoldersAndFilesOnly = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
        public static readonly PropagationFlags PF_SubfoldersAndFilesOnly = PropagationFlags.InheritOnly;

        public static readonly InheritanceFlags IF_SubfoldersAndFilesOnlyX = InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit;
        public static readonly PropagationFlags PF_SubfoldersAndFilesOnlyX = PropagationFlags.InheritOnly;

        public static readonly InheritanceFlags IF_SubfoldersOnly = InheritanceFlags.ContainerInherit;
        public static readonly PropagationFlags PF_SubfoldersOnly = PropagationFlags.InheritOnly;

        public static readonly InheritanceFlags IF_SubfoldersOnlyX = InheritanceFlags.ContainerInherit;
        public static readonly PropagationFlags PF_SubfoldersOnlyX = PropagationFlags.InheritOnly | PropagationFlags.NoPropagateInherit;

        public static readonly InheritanceFlags IF_FilesOnly = InheritanceFlags.ObjectInherit;
        public static readonly PropagationFlags PF_FilesOnly = PropagationFlags.InheritOnly;

        public static readonly InheritanceFlags IF_FilesOnlyX = InheritanceFlags.ObjectInherit;
        public static readonly PropagationFlags PF_FilesOnlyX = PropagationFlags.InheritOnly | PropagationFlags.NoPropagateInherit;

        /// <summary>
        /// Checks to see if a directory has inheritance enabled
        /// </summary>
        /// <param name="dInfo">DirectoryInfo object to check</param>
        /// <returns></returns>
        public static bool HasInheritanceEnabled(DirectoryInfo dInfo)
        {
            var dSecurity = dInfo.GetAccessControl();
            byte[] rawBytes = dSecurity.GetSecurityDescriptorBinaryForm();
            RawSecurityDescriptor rsd = new RawSecurityDescriptor(rawBytes, 0);

            if ((rsd.ControlFlags & ControlFlags.DiscretionaryAclProtected) == ControlFlags.DiscretionaryAclProtected)
            {
                // "Include inheritable permissions from this object's parent" is unchecked
                return false;
            }
            else
            {
                // "Include inheritable permissons from this object's parent" is checked
                return true;
            }
        }
    }

}
