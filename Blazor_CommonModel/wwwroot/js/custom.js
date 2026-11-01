
function editNameLabel() {
    try {
        stickService.innerHTML = (selectService.value ?? '---')
        let text = selectService.value + (selectMarka.value ? ' для ' + selectMarka.value : ' ') + ' ' + selectModel.value + ' ' + selectGeneration.value;
        document.getElementById('nameService').innerHTML = (text.trim() !== '' ? text + ' - ' : ' ');
    } catch(E) {}
}
editNameLabel();