import {makeAutoObservable} from "mobx";

export default class PackageStore {
    constructor() {
        this._packages = []
        this._selectedPack = {}
        makeAutoObservable(this)
    }

    setPackages(packages) {
        this._packages = packages
    }
    setSelectedPack(pack) {
        this._selectedPack = pack
    }
   

    get packages() {
        return this._packages
    }
    get selectedPack() {
        return this._selectedPack
    }
}
