async function getFirebaseConfig() {
    try {
        const response = await fetch('/Firebase/GetConfig');
        const firebaseConfig = await response.json();
        return firebaseConfig;
    } catch (error) {
        console.error("Failed to fetch Firebase configuration:", error);
        throw error;
    }
}

async function initializeFirebase() {
    try {
        const firebaseConfig = await getFirebaseConfig();
        firebase.initializeApp(firebaseConfig);
    } catch (error) {
        console.error("Firebase initialization failed:", error);
    }
}

async function loginWithGoogle() {
    try {
        await initializeFirebase();
        const provider = new firebase.auth.GoogleAuthProvider();
        const result = await firebase.auth().signInWithPopup(provider);
        const user = result.user;
        const idToken = await user.getIdToken();

        const response = await fetch('/Login/LoginWithGoogle', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ token: idToken })
        });

        const data = await response.json();
        if (data.success) {
            Swal.fire({
                icon: 'success',
                title: 'Đăng nhập thành công',
                showConfirmButton: false,
                timer: 2000
            }).then(() => {
                window.location.href = "/";
            });
        } else {
            alert("Đăng nhập thất bại!");
        }
    } catch (error) {
        console.error("Login with Google failed:", error);
    }
}