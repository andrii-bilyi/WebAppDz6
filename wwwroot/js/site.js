document.addEventListener('DOMContentLoaded', () => {
    //вхід
    const authButton = document.getElementById("auth-button");    
    if (authButton) authButton.addEventListener('click', authButtonClick);  
    //вихід
    const signoutButton = document.getElementById("auth-signout-button");
    if (signoutButton) signoutButton.addEventListener('click', signOutButtonClick);

    //зміни
    const saveProfileButton = document.getElementById("profile-save-button");
    if (saveProfileButton) saveProfileButton.addEventListener('click', saveProfileButtonClick);

    //видалення
    const deleteProfileButton = document.getElementById("profile-delete-button");
    if (deleteProfileButton) deleteProfileButton.addEventListener('click', deleteProfileButtonClick);

    //м'яке видалення
    const softRemovalProfileButton = document.getElementById("profile-softRemoval-button");
    if (softRemovalProfileButton) softRemovalProfileButton.addEventListener('click', softRemovalProfileButtonClick);
});

function softRemovalProfileButtonClick() {

    //видалення аккаунту
    fetch(`/user/softRemovalProfile`, { method: 'POST' })
        .then(r => {
            if (r.status === 200) {
                //вихід після видалення
                fetch(`/api/auth`, { method: 'DELETE' })
                    .then(r => {
                        if (r.status === 200) {
                            // Переадресація на головну сторінку
                            window.location.href = '/Home';
                        } else {
                            //showAuthMessage("Помилка виходу");
                            confirm("Помилка виходу!");
                        }
                    });
                confirm("Аккаунт успішно призупинено!");
            } else {
                confirm("Помилка видалення!");
            }

        });
}

function deleteProfileButtonClick() {

    //видалення аккаунту
    fetch(`/user/DeleteProfile`, { method: 'DELETE' })
        .then(r => {
            if (r.status === 200) {
                //вихід після видалення
                fetch(`/api/auth`, { method: 'DELETE' })
                    .then(r => {
                        if (r.status === 200) {
                            // Переадресація на головну сторінку
                            window.location.href = '/Home';                            
                        } else {
                            //showAuthMessage("Помилка виходу");
                            confirm("Помилка виходу!");
                        }
                    });   
                confirm("Аккаунт успішно видалено!");
            } else {                
                confirm("Помилка видалення!");
            }

        }); 
}

function saveProfileButtonClick() {
    const nameInput = document.querySelector('input[name="profile-name"]');
    if (!nameInput) throw 'Element input[name="profile-name"] not found';
    const emailInput = document.querySelector('input[name="profile-email"]');
    if (!emailInput) throw 'Element input[name="profile-email"] not found';
    fetch(
        `/user/UpdateProfile?newName=${nameInput.value}&newEmail=${emailInput.value}`,
        {
            method: 'POST'
        }
    )
        .then(r => r.json())
        .then(j => {
            console.log(j);
        });
}

function signOutButtonClick() {
    const confirmSignOut = confirm("Ви впевнені, що хочете вийти?");
    if (confirmSignOut) {
        fetch(`/api/auth`, { method: 'DELETE' })
            .then(r => {
                if (r.status === 200) {     
                    //// Перезавантаження поточної сторінки
                    //window.location.reload();

                    // Переадресація на головну сторінку
                    window.location.href = '/Home';
                    confirm("Вихід виконано!");
                } else {
                    //showAuthMessage("Помилка виходу");
                    confirm("Помилка виходу!");
                }
            });
    }
}

function authButtonClick() {
    const loginInput = document.getElementById("auth-login");
    if (!loginInput) throw "Element #auth-login not found";
    const login = loginInput.value.trim();
    if (login.length == 0) {
        showAuthMessage("Логін не може бути порожнім");
        return;
    }
    const passwordInput = document.getElementById("auth-password");
    if (!passwordInput) throw "Element #auth-password not found";
    const password = passwordInput.value.trim();
    if (password.length == 0) {
        showAuthMessage("Пароль не може бути порожнім");
        return;
    }
    
    fetch(`/api/auth?login=${login}&password=${password}`)
        .then(r => {
            if (r.status == 200) { // ok
                window.location.reload();
                confirm("Вхід виконано!");                
            }
            else if (r.status == 410) {
                showAuthMessage("Аккаунт призупинено!");
            }
            else { // 401
                showAuthMessage("Вхід відхилено!");
            }
        });
    //    r.json())
    //.then(j => {
    //    console.log(j);
    //    showAuthMessage(j.status);
    //});
}
function showAuthMessage(message) {
    const authMessage = document.getElementById("auth-message");
    if (!authMessage) throw "Element #auth-message not found";
    authMessage.innerText = message;
    authMessage.classList.remove("visually-hidden");
}

