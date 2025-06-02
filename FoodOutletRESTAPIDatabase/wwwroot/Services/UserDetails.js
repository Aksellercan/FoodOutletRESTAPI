export default class UserDetails {
    constructor() {
        this.isLoggedin = false;
        this.CurrentUserId = null;
        this.CurrentUsername = null;
    }

    setUserDetails(CurrentUserId, CurrentUsername) {
        this.CurrentUserId = CurrentUserId;
        this.CurrentUsername = CurrentUsername;
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
}