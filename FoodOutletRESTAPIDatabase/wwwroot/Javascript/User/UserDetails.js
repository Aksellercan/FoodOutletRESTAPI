export default class UserDetails {
    constructor() {
        this.isLoggedin = false;
        this.CurrentUserId = null;
        this.CurrentUsername = null;
        this.CurrentUserRole = null;
    }

    setUserDetails(CurrentUserId, CurrentUsername, CurrentUserRole) {
        this.CurrentUserId = CurrentUserId;
        this.CurrentUsername = CurrentUsername;
        this.CurrentUserRole = CurrentUserRole
    }

    setLoggedinStatus(isLoggedin) {
        this.isLoggedin = isLoggedin;
    }

    resetUserState() {
        this.isLoggedin = false;
        this.CurrentUserId = null;
        this.CurrentUsername = null;
    }

    getisLoggedin() {
        return this.isLoggedin;
    }
    getCurrentUsername() {
        return this.CurrentUsername
    }
    getCurrentUserId() {
        return this.CurrentUserId;
    }
    getCurrentUserRole(){
        return this.CurrentUserRole;
    }
}