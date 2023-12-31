import {Grid} from "semantic-ui-react";
import EventList from "./EventList";
import EventFilters from "./EventFilters";
import {useStore} from "../../../app/stores/store";
import EventPagination from './EventPagination';
import { useEffect } from "react";
import { observer } from "mobx-react-lite";
import { toast } from 'react-toastify';
import LoadingComponent from "../../../app/common/LoadingComponent";
import LoginForm from "../../users/LoginForm";
import LoginFooter from "../../users/LoginFooter";

function EventDashboard() {
    const { eventStore, accountStore, modalStore } = useStore();

    // useEffect(() => {
    //     if (eventStore.eventRegistry.size === 0) {
    //         eventStore.loadEvents(eventStore.pageNumber);
    //     }
    // }, [eventStore.totalPages]);
    useEffect(() => {
        eventStore.loading = true;
        eventStore.loadEvents(eventStore.pageNumber);
    }, []);

    useEffect(() => {
        if(accountStore.redirectToLoginModal){
            if(accountStore.isLoggedIn){
                toast.warn("You don't have an access to this page!");
                accountStore.setRedirectToLoginModal(false);
            } else {
                modalStore.openModal(<LoginForm/>, "Login to help others", <LoginFooter/>);
            }
        }
    }, [accountStore, modalStore]);

    if (eventStore.loading) return <LoadingComponent content={'Loading events'} />;

    return (
        <Grid>
            <Grid.Column style={{marginTop:60}} width={11}>

                <EventList/>
                { eventStore.totalPages > 1 ? <EventPagination/> : null}
            </Grid.Column>

            <Grid.Column style={{marginLeft: 50, marginTop:60}} width={3}>
                <EventFilters/>
            </Grid.Column>
        </Grid>
    );
}

export default observer(EventDashboard);