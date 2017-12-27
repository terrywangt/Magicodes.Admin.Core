using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Abp.AutoMapper;
using Magicodes.Admin.Authorization.Users.Dto;
using Xamarin.Forms;

namespace Magicodes.Admin.Models.Users
{
    [AutoMapFrom(typeof(GetUserForEditOutput))]
    public class UserEditOrCreateModel : GetUserForEditOutput, INotifyPropertyChanged
    {
        private ImageSource _photo;
        private List<OrganizationUnitModel> _organizationUnits;

        public string FullName => User == null ? string.Empty : User.Name + " " + User.Surname;

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public ImageSource Photo
        {
            get => _photo;
            set
            {
                _photo = value;
                RaisePropertyChanged(nameof(Photo));
            }
        }

        public List<OrganizationUnitModel> OrganizationUnits
        {
            get => _organizationUnits;
            set
            {
                _organizationUnits = value?.OrderBy(o => o.Code).ToList();
                SetAsAssignedForMemberedOrganizationUnits();
                RaisePropertyChanged(nameof(OrganizationUnits));
            }
        } 

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SetAsAssignedForMemberedOrganizationUnits()
        {
            if (_organizationUnits != null)
            {
                MemberedOrganizationUnits?.ForEach(memberedOrgUnitCode =>
                {
                    _organizationUnits
                        .Single(o => o.Code == memberedOrgUnitCode)
                        .IsAssigned = true;
                });
            }
        }
    }
}