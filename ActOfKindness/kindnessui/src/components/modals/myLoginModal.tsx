import React from 'react';
import {Modal, Segment} from "semantic-ui-react";
import {store, useStore} from "../../app/stores/store";
import {observer} from "mobx-react-lite";

function MyLoginModal() {
    const { modalStore} = useStore()

    return (
        <>
            <Modal
                dimmer={'blurring'}
                open={modalStore.modal.open}
                onClose={modalStore.closeModal}
                size={'mini'}
            >
                <Modal.Header  style={{textAlign:'center', color: 'green', fontSize:'1.5em'}}>Login </Modal.Header>
                <Modal.Content>
                    {modalStore.modal.body}
                </Modal.Content>
            </Modal>
        </>
    );
}

export default observer(MyLoginModal)