const uri = 'http://localhost:53599/api/contacts';
let contacts = [];

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Unable to get contacts.', error));
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'contact' : 'contacts';
        document.getElementById('counter').innerText = `${itemCount} ${name}`;
    }

    function _displayItems(data) {
        const tBody = document.getElementById('contacts');
        tBody.innerHTML = '';

        _displayCount(data.length);

        const button = document.createElement('button');

        data.forEach(item => {

            let editButton = button.cloneNode(false);
            editButton.innerText = 'Edit';
            editButton.setAttribute('onclick', `displayEditForm(${item.id})`);

            let deleteButton = button.cloneNode(false);
            deleteButton.innerText = 'Delete';
            deleteButton.setAttribute('onclick', `deleteItem(${item.id})`);

            let tr = tBody.insertRow();

            let td0 = tr.insertCell(0);
            let textNodeId = document.createTextNode(item.contactId);
            td0.appendChild(textNodeId);

            let td1 = tr.insertCell(1);
            let textNodeName = document.createTextNode(item.name);
            td1.appendChild(textNodeName);

            let td2 = tr.insertCell(2);
            let textNodeDob = document.createTextNode(item.dateOfBirth);
            td2.appendChild(textNodeDob);

            let td3 = tr.insertCell(3);
            let textNodeAddress = document.createTextNode(item.address);
            td3.appendChild(textNodeAddress);

            let td4 = tr.insertCell(4);
            td4.appendChild(editButton);

            let td5 = tr.insertCell(5);
            td5.appendChild(deleteButton);
        });

        contacts = data;
    }
